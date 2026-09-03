using MeuPetShop.Domain.Dtos.ClientDtos;
using MeuPetShop.Domain.Dtos.PetDtos;
using MeuPetShop.Domain.Interfaces.IPets;
using MeuPetShop.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeuPetshop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetsController : ControllerBase
{
    private readonly IPetService _petService;

    public PetsController(IPetService petService)
    {
        _petService = petService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<PetDto>>> GetAllPets([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await _petService.GetAllPetsAsync(pageNumber, pageSize);
        return Ok(response);
    }


    [HttpGet("/api/clients/{clientId}/pets")]
    public async Task<ActionResult<IEnumerable<PetDto>>> GetAllPetsForClient(int clientId)
    {
        try
        {
            var pets = await _petService.GetAllPetsForClientAsync(clientId);
            return Ok(pets);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("/api/clients/{clientId}/pets")]
    public async Task<ActionResult<PetDto>> CreatePetForClient(int clientId, [FromBody] CreatePetDto petDto)
    {
        try
        {
            var newPet = await _petService.CreatePetForClientAsync(clientId, petDto);
            return CreatedAtRoute("GetPetById", new { id = newPet.Id }, newPet);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id}", Name = "GetPetById")]
    public async Task<ActionResult<PetDto>> GetPetById(int id)
    {
        var pet = await _petService.GetPetByIdAsync(id);
        if (pet == null)
        {
            return NotFound($"Pet com ID {id} não encontrado.");
        }

        return Ok(pet);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PetDto>> UpdatePet(int id, [FromBody] UpdatePetDto petDto)
    {
        var updatedPet = await _petService.UpdatePetAsync(id, petDto);
        if (updatedPet == null)
        {
            return NotFound($"Pet com ID {id} não encontrado.");
        }

        return Ok(updatedPet);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeletePet(int id)
    {
        var success = await _petService.DeletePetAsync(id);
        if (!success)
        {
            return NotFound($"Pet com ID {id} não encontrado.");
        }

        return NoContent();
    }
}