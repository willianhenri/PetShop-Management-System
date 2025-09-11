using MeuPetShop.Domain.Dtos.ClientDtos;
using MeuPetShop.Domain.Shared;

namespace MeuPetShop.Domain.Interfaces.IClients;

public interface IClientService
{
    Task<ClientDto> CreateClientAsync(CreateClientDto clientDto);
    Task<ClientDto?> GetClientByIdAsync(int id);
    Task<PagedApiResponse<ClientDto>> GetAllClientsAsync(int pageNumber, int pageSize);
    Task<ClientDto?> UpdateClientAsync(int id, UpdateClientDto clientDto);
    Task<bool> DeleteClientAsync(int id);
}