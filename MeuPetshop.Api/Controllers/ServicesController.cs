using MeuPetShop.Domain.Dtos.ServiceDtos;
using MeuPetShop.Domain.Interfaces.IService;
using MeuPetShop.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeuPetshop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServicesController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }
    
    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<ServiceDto>>> GetAllServices([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _serviceService.GetAllServicesAsync(pageNumber, pageSize);
        return Ok(response);
    }

    [HttpGet("{id}", Name = "GetServiceById")]
    public async Task<ActionResult<ServiceDto>> GetServiceById(int id)
    {
        var service = await _serviceService.GetServiceByIdAsync(id);
        if (service == null) return NotFound();
        return Ok(service);
    }
    
    [HttpPost]
    public async Task<ActionResult<ServiceDto>> CreateService([FromBody] CreateServiceDto serviceDto)
    {
        var newService = await _serviceService.CreateServiceAsync(serviceDto);
        return CreatedAtRoute("GetServiceById", new { id = newService.Id }, newService);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ServiceDto>> UpdateService(int id, [FromBody] UpdateServiceDto serviceDto)
    {
        var updatedService = await _serviceService.UpdateServiceAsync(id, serviceDto);
        if (updatedService == null)
        {
            return NotFound();
        }
        return Ok(updatedService);
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var success = await _serviceService.DeleteServiceAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}