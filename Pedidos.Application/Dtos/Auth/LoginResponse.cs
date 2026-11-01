namespace Pedidos.Application.Dtos.Auth;

public sealed record LoginResponse(string Token, int ExpiresIn);
