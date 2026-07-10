using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;
using MeuPetShop.Domain.Dtos.Auth;
using MeuPetShop.Domain.Entities.User;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MeuPetshop.Domain.Dtos;

namespace MeuPetshop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var userExists = await _userManager.FindByNameAsync(registerDto.UserName);
        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status409Conflict, new { Message = "Username já existe." });
        }

        var emailExists = await _userManager.FindByEmailAsync(registerDto.Email);
        if (emailExists != null)
        {
            return StatusCode(StatusCodes.Status409Conflict, new { Message = "Email já cadastrado." });
        }

        ApplicationUser user = new()
        {
            Email = registerDto.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerDto.UserName,
            FullName = registerDto.FullName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Falha ao criar usuário.", Errors = errors });
        }

        string roleToAssign;
    
        if (registerDto.Email == "admin@petshop.com")
        {
            roleToAssign = "Admin";
        }
        else
        {
            roleToAssign = "Funcionario";
        }

        if (!await _roleManager.RoleExistsAsync(roleToAssign))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleToAssign));
        }
    
        await _userManager.AddToRoleAsync(user, roleToAssign);

        return Ok(new { Message = "Usuário criado com sucesso!" });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName);

        if (user != null && user.IsActive && await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            var token = await GenerateJwtToken(user);
            return Ok(new LoginResponseDto(token));
        }
        
        return Unauthorized(new { Message = "Usuário ou senha inválidos."});
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };
        
        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApiSettings:Secret"]));

        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            expires: DateTime.UtcNow.AddHours(8),
            claims: claims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("forgot-password")]
    
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
       
        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        
        if (user == null)
        {
           
            return Ok(new { Message = "Se o e-mail existir em nossa base, um token de recuperação será enviado." });
        }

       
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        
        var apiKey = _configuration["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);

        
        var from = new EmailAddress("willian.h1227@gmail.com", "Meu PetShop");
        var subject = "Redefinição de Senha - Meu PetShop";
        
       
        var to = new EmailAddress(user.Email, user.FullName);

       
        var resetLink = $"https://pet-shop-front-ecru.vercel.app/reset-password?token={Uri.EscapeDataString(resetToken)}&email={user.Email}";
        
       
        var plainTextContent = $"Clique no link para redefinir sua senha: {resetLink}";
        var htmlContent = $"<h2>Recuperação de Senha</h2><p>Olá {user.FullName},</p><p>Para escolher uma nova senha, clique no link abaixo:</p><p><strong><a href='{resetLink}'>Redefinir minha senha</a></strong></p><p>Se não solicitou esta alteração, pode ignorar este e-mail.</p>";
        
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        
        
        var response = await client.SendEmailAsync(msg);

        if (response.IsSuccessStatusCode)
        {
            return Ok(new { Message = "Se o e-mail existir em nossa base, um token de recuperação será enviado." });
        }
        else
        {
            var errorBody = await response.Body.ReadAsStringAsync();
            Console.WriteLine($"ERRO SENDGRID: Status {response.StatusCode} - Detalhes: {errorBody}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Erro ao tentar enviar o e-mail de recuperação." });
        }
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return BadRequest("Usuário não encontrado.");
        }

        
        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

        
        if (result.Succeeded)
        {
            return Ok("Senha alterada com sucesso!");
        }

        
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return BadRequest(errors);
    }


    [HttpGet]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> GetAllUsers()
    {
        var users = _userManager.Users.ToList();

        
        var userList = users.Select(user => new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.FullName,
            user.IsActive,
            user.CreatedAt
        }).ToList();

        return Ok(userList);
    }

    [HttpGet("{UserName}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserByUserName(string UserName)
    {
        var user = await _userManager.FindByNameAsync(UserName);
        if (user == null)
        {
            return NotFound(new { Message = "Usuário não encontrado." });
        }

        var userDetails = new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.FullName,
            user.IsActive,
            user.CreatedAt
        };

        return Ok(userDetails);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> DeleteUser(string id)
    {
        
        var user = await _userManager.FindByIdAsync(id);
        
        if (user == null)
        {
            return NotFound(new { Message = "Usuário não encontrado." });
        }

       
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (user.Id == currentUserId)
        {
            return BadRequest(new { Message = "Você não pode excluir sua própria conta." });
        }

       
        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Erro ao excluir usuário.", Errors = errors });
        }

        return Ok(new { Message = "Usuário excluído com sucesso!" });
    }
    
}
