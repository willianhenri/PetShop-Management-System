using MeuPetShop.Domain.Dtos.AppointmentDto;
using MeuPetShop.Domain.Interfaces.IAppointment;
using MeuPetShop.Domain.Interfaces.IService;
using MeuPetShop.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeuPetshop.Api.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize]
public class AppointmentsController : Controller
{
     private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet("{id}", Name = "GetAppointmentById")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetAppointmentById(int id)
    {
        var response = await _appointmentService.GetAppointmentByIdAsync(id);
        if (!response.Success)
        {
            return NotFound(response);
        }
        return Ok(response);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedApiResponse<AppointmentDto>>> FindAppointmentsByDate(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        var response = await _appointmentService.FindAppointmentsByDateRangeAsync(startDate, endDate, pageNumber, pageSize);
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> CreateAppointment([FromBody] CreateAppointmentDto appointmentDto)
    {
        var response = await _appointmentService.CreateAppointmentAsync(appointmentDto);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return CreatedAtRoute("GetAppointmentById", new { id = response.Data.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> UpdateAppointment(int id, [FromBody] UpdateAppointmentDto appointmentDto)
    {
        var response = await _appointmentService.UpdateAppointmentAsync(id, appointmentDto);
        if (!response.Success)
        {
            return NotFound(response);
        }
        return Ok(response);
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> CancelAppointment(int id)
    {
        var response = await _appointmentService.CancelAppointmentAsync(id);
        if (!response.Success)
        {
            return NotFound(response);
        }
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<PagedApiResponse<AppointmentDto>>> GetAllAppointments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _appointmentService.GetAllAppointmentsAsync(pageNumber, pageSize);
        return Ok(response);
    }
}