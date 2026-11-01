using Pedidos.Domain.Constants;

namespace Pedidos.Domain.Entities;

public sealed class Usuario
{
    public int Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Rol { get; private set; } = null!;
    public bool Activo { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    private Usuario()
    {
    }

    public bool EsAdmin => Rol == Roles.Admin;
}
