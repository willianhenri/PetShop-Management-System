using MeuPetShop.Domain.Dtos.ClientDtos;
using MeuPetShop.Domain.Dtos.PetDtos;
using MeuPetShop.Domain.Entities;
using MeuPetShop.Domain.Interfaces.IClients;
using MeuPetShop.Domain.Shared;

namespace MeuPetshop.Application.Services;

public class ClientServices : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientServices(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    public async Task<ClientDto> CreateClientAsync(CreateClientDto clientDto)
    {
        var existingClient = await _clientRepository.GetByEmailAsync(clientDto.Email);
        if (existingClient != null)
        {
            throw new InvalidOperationException("Já existe um cliente cadastrado com este e-mail.");
        }

        var newClient = new Client
        {
            Name = clientDto.Name,
            Email = clientDto.Email,
            Phone = clientDto.Phone,
            Address = clientDto.Adress
        };
        
        await _clientRepository.AddAsync(newClient);
        return MapClientToDto(newClient);
    }

    public async Task<ClientDto?> GetClientByIdAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client == null) throw new KeyNotFoundException($"Client {id} not found.");
        
        return MapClientToDto(client);
    }

    public async Task<PagedApiResponse<ClientDto>> GetAllClientsAsync(int pageNumber, int pageSize)
    {
        var (clients, totalCount) = await _clientRepository.GetAllAsync(pageNumber, pageSize);
        var clientDtos = clients.Select(MapClientToDto);
        
        var response = new PagedApiResponse<ClientDto>
        {
            Data = clientDtos,
            Pagination = new PaginationData
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
        return response;
    }


    public async Task<ClientDto?> UpdateClientAsync(int id, UpdateClientDto clientDto)
    {
        var clientToUpdate = await _clientRepository.GetByIdAsync(id);
        if (clientToUpdate == null) throw new KeyNotFoundException($"Client {id} not found.");
        
        var existingClientWithEmail = await _clientRepository.GetByEmailAsync(clientDto.Email);
        if (existingClientWithEmail != null && existingClientWithEmail.Id != id)
            throw new InvalidOperationException("O email informado ja está em uso");
        clientToUpdate.Name = clientDto.Name;
        clientToUpdate.Email = clientDto.Email;
        clientToUpdate.Phone = clientDto.Phone;
        clientToUpdate.Address = clientDto.Adress;
        await _clientRepository.UpdateAsync(clientToUpdate);
        return MapClientToDto(clientToUpdate);
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        var clientToDelete = await _clientRepository.GetByIdAsync(id);
        if (clientToDelete == null) throw new KeyNotFoundException($"Client {id} not found.");
        await _clientRepository.DeleteAsync(clientToDelete );
        return true;
    }
    
    
    private ClientDto MapClientToDto(Client client)
    {
        var petsDto = client.Pets?.Select(pet => new PetDto(pet.Id, pet.Name, pet.Breed, pet.Specie, pet.ClientId)).ToList() 
                      ?? new List<PetDto>();

        return new ClientDto(client.Id, client.Name, client.Phone, client.Email, client.Address, petsDto);
    }
}