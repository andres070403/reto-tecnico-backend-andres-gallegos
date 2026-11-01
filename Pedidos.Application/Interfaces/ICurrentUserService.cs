namespace Pedidos.Application.Interfaces;

public interface ICurrentUserService
{
    int? UsuarioId { get; }
    string? Email { get; }
    string? Rol { get; }
    bool EstaAutenticado { get; }
}
