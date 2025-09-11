using MeuPetShop.Domain.Dtos.ServiceDtos;
using MeuPetShop.Domain.Shared;

namespace MeuPetShop.Domain.Interfaces.IService;

public interface IServiceService
{
    Task<ServiceDto> CreateServiceAsync(CreateServiceDto serviceDto);
    Task<ServiceDto?> GetServiceByIdAsync(int id);
    Task<PagedApiResponse<ServiceDto>> GetAllServicesAsync(int pageNumber, int pageSize);
    Task<ServiceDto?> UpdateServiceAsync(int id, UpdateServiceDto serviceDto);
    Task<bool> DeleteServiceAsync(int id);
}