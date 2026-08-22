using System.Runtime.InteropServices.JavaScript;

namespace MeuPetShop.Domain.Shared;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}