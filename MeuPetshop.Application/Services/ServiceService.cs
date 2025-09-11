using MeuPetShop.Domain.Dtos.ServiceDtos;
using MeuPetShop.Domain.Entities;
using MeuPetShop.Domain.Interfaces.IService;
using MeuPetShop.Domain.Shared;

namespace MeuPetshop.Application.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _repository;

    public ServiceService(IServiceRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ServiceDto> CreateServiceAsync(CreateServiceDto serviceDto)
    {
        var newService = new Service
        {
            Name = serviceDto.Name,
            Description = serviceDto.Description,
            Price = serviceDto.Price,
            DurationInMinutes = serviceDto.DurationInMinutes
        };
        await _repository.AddAsync(newService);
        return MapServiceToDto(newService);
    }

    public async Task<ServiceDto?> GetServiceByIdAsync(int id)
    {
        var service = await _repository.GetByIdAsync(id);
        if (service == null) return null;
        return MapServiceToDto(service);
    }

    public async Task<PagedApiResponse<ServiceDto>> GetAllServicesAsync(int pageNumber, int pageSize)
    {
        var (services, totalCount) = await _repository.GetAllAsync(pageNumber, pageSize);
        var serviceDtos = services.Select(MapServiceToDto);

        var response = new PagedApiResponse<ServiceDto>
        {
            Data = serviceDtos,
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


    public async Task<ServiceDto?> UpdateServiceAsync(int id, UpdateServiceDto serviceDto)
    {
        var service = await _repository.GetByIdAsync(id);
        if (service == null) return null;
        
        service.Name = serviceDto.Name;
        service.Description = serviceDto.Description;
        service.Price = serviceDto.Price;
        service.DurationInMinutes = serviceDto.DurationInMinutes;
        return MapServiceToDto(service);
    }

    public async Task<bool> DeleteServiceAsync(int id)
    {
        var service = await _repository.GetByIdAsync(id);
        if (service == null) return false;
        await _repository.DeleteAsync(service);
        return true;
    }
    
    private ServiceDto MapServiceToDto(Service service)
    {
        return new ServiceDto(service.Id, service.Name, service.Description, service.Price, service.DurationInMinutes);
    }
}