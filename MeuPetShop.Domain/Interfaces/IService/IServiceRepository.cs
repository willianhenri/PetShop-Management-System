using MeuPetShop.Domain.Entities;

namespace MeuPetShop.Domain.Interfaces.IService;

public interface IServiceRepository
{
    Task AddAsync(Service service);
    Task<Service?> GetByIdAsync(int id);
    Task<(IEnumerable<Service> Services, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    Task UpdateAsync(Service service);
    Task DeleteAsync(Service service);

}