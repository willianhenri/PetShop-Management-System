using MeuPetShop.Domain.Entities;

namespace MeuPetShop.Domain.Interfaces.IClients;

public interface IClientRepository
{
    Task AddAsync(Client client);
    Task<Client?> GetByIdAsync(int id);
    Task<(IEnumerable<Client> Clients, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    Task<Client?> GetByEmailAsync(string email); 
    Task UpdateAsync(Client client);
    Task DeleteAsync(Client client);
}