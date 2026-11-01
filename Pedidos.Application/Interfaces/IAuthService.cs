using Pedidos.Application.Dtos.Auth;

namespace Pedidos.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
