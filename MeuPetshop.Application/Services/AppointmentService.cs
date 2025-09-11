using MeuPetShop.Domain.Dtos.AppointmentDto;
using MeuPetShop.Domain.Dtos.ClientDtos;
using MeuPetShop.Domain.Dtos.PetDtos;
using MeuPetShop.Domain.Dtos.ServiceDtos;
using MeuPetShop.Domain.Entities;
using MeuPetShop.Domain.Interfaces.IAppointment;
using MeuPetShop.Domain.Interfaces.IClients;
using MeuPetShop.Domain.Interfaces.IPets;
using MeuPetShop.Domain.Interfaces.IService;
using MeuPetShop.Domain.Shared;

namespace MeuPetshop.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IPetRepository _petRepository;
    private readonly IServiceRepository _serviceRepository;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IClientRepository clientRepository,
        IPetRepository petRepository,
        IServiceRepository serviceRepository)
    {
        _appointmentRepository = appointmentRepository;
        _clientRepository = clientRepository;
        _petRepository = petRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<PagedApiResponse<AppointmentDto>> FindAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
    {
        var (appointments, totalCount) = await _appointmentRepository.FindByDateRangeAsync(startDate.ToUniversalTime(), endDate.ToUniversalTime(), pageNumber, pageSize);
        var appointmentDtos = appointments.Select(MapAppointmentToDto);

        return new PagedApiResponse<AppointmentDto>
        {
            Data = appointmentDtos,
            Pagination = new PaginationData
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
    }
    
    // Todos os métodos abaixo foram atualizados para retornar ApiResponse<T>
    public async Task<ApiResponse<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto appointmentDto)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(appointmentDto.ClientId);
            var pet = await _petRepository.GetByIdAsync(appointmentDto.PetId);
            var service = await _serviceRepository.GetByIdAsync(appointmentDto.ServiceId);

            if (client == null || pet == null || service == null)
            {
                return new ApiResponse<AppointmentDto> { Success = false, Message = "Cliente, Pet ou Serviço não encontrado." };
            }
            if (pet.ClientId != client.Id)
            {
                return new ApiResponse<AppointmentDto> { Success = false, Message = "O pet informado não pertence ao cliente informado." };
            }
            
            var newAppointment = new Appointment
            {
                ClientId = appointmentDto.ClientId,
                PetId = appointmentDto.PetId,
                ServiceId = appointmentDto.ServiceId,
                AppointmentDateTime = appointmentDto.AppointmentDateTime.ToUniversalTime(),
                Notes = appointmentDto.Notes,
                AppointmentStatus = AppointmentStatus.Scheduled
            };

            await _appointmentRepository.AddAsync(newAppointment);
            var createdAppointment = await _appointmentRepository.GetByIdAsync(newAppointment.Id);
            
            return new ApiResponse<AppointmentDto> { Data = MapAppointmentToDto(createdAppointment!) };
        }
        catch (Exception ex)
        {
            return new ApiResponse<AppointmentDto> { Success = false, Message = "Ocorreu um erro ao criar o agendamento.", Errors = new List<string> { ex.Message } };
        }
    }

    public async Task<ApiResponse<AppointmentDto>> GetAppointmentByIdAsync(int id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            return new ApiResponse<AppointmentDto> { Success = false, Message = $"Agendamento com ID {id} não encontrado." };
        }
        return new ApiResponse<AppointmentDto> { Data = MapAppointmentToDto(appointment) };
    }
    
    public async Task<ApiResponse<AppointmentDto>> UpdateAppointmentAsync(int id, UpdateAppointmentDto appointmentDto)
    {
        var appointmentToUpdate = await _appointmentRepository.GetByIdAsync(id);
        if (appointmentToUpdate == null)
        {
            return new ApiResponse<AppointmentDto> { Success = false, Message = $"Agendamento com ID {id} não encontrado." };
        }

        appointmentToUpdate.AppointmentDateTime = appointmentDto.AppointmentDateTime.ToUniversalTime();
        appointmentToUpdate.AppointmentStatus = appointmentDto.Status;
        appointmentToUpdate.Notes = appointmentDto.Notes;

        await _appointmentRepository.UpdateAsync(appointmentToUpdate);

        return new ApiResponse<AppointmentDto> { Data = MapAppointmentToDto(appointmentToUpdate) };
    }
    
    public async Task<ApiResponse<AppointmentDto>> CancelAppointmentAsync(int id)
    {
        var appointmentToCancel = await _appointmentRepository.GetByIdAsync(id);
        if (appointmentToCancel == null)
        {
            return new ApiResponse<AppointmentDto> { Success = false, Message = $"Agendamento com ID {id} não encontrado." };
        }

        appointmentToCancel.AppointmentStatus = AppointmentStatus.Canceled;
        await _appointmentRepository.UpdateAsync(appointmentToCancel);
        
        return new ApiResponse<AppointmentDto> { Data = MapAppointmentToDto(appointmentToCancel) };
    }
    

    public async Task<PagedApiResponse<AppointmentDto>> GetAllAppointmentsAsync(int pageNumber, int pageSize)
    {
        var (appointments, totalCount) = await _appointmentRepository.GetAllAsync(pageNumber, pageSize);
        var appointmentDtos = appointments.Select(MapAppointmentToDto);

        return new PagedApiResponse<AppointmentDto>
        {
            Data = appointmentDtos,
            Pagination = new PaginationData
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        };
    }
    
    private AppointmentDto MapAppointmentToDto(Appointment appointment)
    {
        var clientDto = new ClientDto(appointment.Client.Id, appointment.Client.Name, appointment.Client.Phone, appointment.Client.Email, appointment.Client.Address, new List<PetDto>());
        var petDto = new PetDto(appointment.Pet.Id, appointment.Pet.Name, appointment.Pet.Breed, appointment.Pet.Specie, appointment.Pet.ClientId);
        var serviceDto = new ServiceDto(appointment.Service.Id, appointment.Service.Name, appointment.Service.Description, appointment.Service.Price, appointment.Service.DurationInMinutes);

        return new AppointmentDto(appointment.Id, appointment.AppointmentDateTime, appointment.AppointmentStatus, appointment.Notes, clientDto, petDto, serviceDto);
    }
    
}