using MeuPetShop.Domain.Entities.User;

namespace MeuPetShop.Domain.Entities;

public class Appointment
{
    public int Id { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    
    public int PetId { get; set; }
    public Pet Pet { get; set; }
    
    public int ServiceId { get; set; }
    public Service Service { get; set; }
    
    public int ClientId { get; set; }
    public Client Client { get; set; }
    
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}