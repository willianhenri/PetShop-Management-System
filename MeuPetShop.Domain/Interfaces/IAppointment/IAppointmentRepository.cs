using MeuPetShop.Domain.Entities;

namespace MeuPetShop.Domain.Interfaces.IAppointment;

public interface IAppointmentRepository
{
    Task AddAsync(Appointment appointment);
    Task<Appointment?> GetByIdAsync(int id);
    Task UpdateAsync(Appointment appointment);
    Task<int> CountAsync();
    Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    Task<(IEnumerable<Appointment> Appointments, int TotalCount)> FindByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
}