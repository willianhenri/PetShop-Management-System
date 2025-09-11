using MeuPetShop.Domain.Dtos.AppointmentDto;
using MeuPetShop.Domain.Shared;

namespace MeuPetShop.Domain.Interfaces.IAppointment;

public interface IAppointmentService
{
    Task<ApiResponse<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto appointmentDto);
    Task<ApiResponse<AppointmentDto>> GetAppointmentByIdAsync(int id);
    Task<PagedApiResponse<AppointmentDto>> FindAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
    Task<ApiResponse<AppointmentDto>> UpdateAppointmentAsync(int id, UpdateAppointmentDto appointmentDto);
    Task<ApiResponse<AppointmentDto>> CancelAppointmentAsync(int id);
    Task<PagedApiResponse<AppointmentDto>> GetAllAppointmentsAsync(int pageNumber, int pageSize);
}