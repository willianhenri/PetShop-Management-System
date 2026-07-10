namespace MeuPetShop.Domain.Dtos.Auth;

public record RegisterDto(
    string UserName,
    string Email,
    string Password,
    string FullName,
    string Role 
);