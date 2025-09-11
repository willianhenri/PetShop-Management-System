using MeuPetShop.Domain.Entities;
using MeuPetShop.Domain.Interfaces.IAppointment;
using MeuPetshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeuPetshop.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Appointment> Appointments, int TotalCount)> FindByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
    {
        var query = _context.Appointments
            .Where(a => a.AppointmentDateTime >= startDate && a.AppointmentDateTime <= endDate);

        var totalCount = await query.CountAsync();

        var appointments = await query
            .Include(a => a.Client)
            .Include(a => a.Pet)
            .Include(a => a.Service)
            .OrderBy(a => a.AppointmentDateTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        return (appointments, totalCount);
    }
    
    
    public async Task AddAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _context.Appointments
            .Include(a => a.Client)
            .Include(a => a.Pet)
            .Include(a => a.Service)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Appointments.CountAsync();
    }

    public async Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
    {
        var totalCount = await _context.Appointments.CountAsync();

        var appointments = await _context.Appointments
            .Include(a => a.Client)
            .Include(a => a.Pet)
            .Include(a => a.Service)
            .OrderByDescending(a => a.AppointmentDateTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (appointments, totalCount);
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }
}