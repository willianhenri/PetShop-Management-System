namespace MeuPetShop.Domain.Dtos.Auth;

public record LoginResponseDto(
    string Token,
    string Role
);