using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Pedidos.Application.Interfaces;

namespace Pedidos.Infrastructure.Security;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Usuario => _httpContextAccessor.HttpContext?.User;

    public int? UsuarioId
    {
        get
        {
            var valor = Usuario?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(valor, out var id) ? id : null;
        }
    }

    public string? Email => Usuario?.FindFirst(ClaimTypes.Email)?.Value;

    public string? Rol => Usuario?.FindFirst(ClaimTypes.Role)?.Value;

    public bool EstaAutenticado => Usuario?.Identity?.IsAuthenticated ?? false;
}
