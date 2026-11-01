using Pedidos.Domain.Enums;

namespace Pedidos.Infrastructure.Persistence.Seed;

internal static class SeedData
{
    private static readonly DateTime FechaSeedBase = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

    public static readonly object[] Usuarios =
    {
        new
        {
            Id = 1,
            Nombre = "Administrador",
            Email = "admin@pedidos.com",
            PasswordHash = "$2a$11$6/hvar.9e3218SzBYSruKe135GDRXVLzVtO08r8t0RnYUNvd1kpjm",
            Rol = "Admin",
            Activo = true,
            FechaCreacion = FechaSeedBase
        },
        new
        {
            Id = 2,
            Nombre = "Usuario Demo",
            Email = "user@pedidos.com",
            PasswordHash = "$2a$11$weSJCsNw0/hk1O0jb3YTI.FHbLjUUoDPM3j6ZDZr6d3De3j2.GHXC",
            Rol = "User",
            Activo = true,
            FechaCreacion = FechaSeedBase
        }
    };

    public static readonly object[] Pedidos =
    {
        new
        {
            Id = 1,
            NumeroPedido = "PE-001",
            Cliente = "Juan Perez",
            Fecha = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Unspecified),
            Total = 250.75m,
            Estado = EstadoPedido.Registrado,
            Eliminado = false,
            UsuarioCreacionId = 1,
            FechaCreacion = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Unspecified),
            UsuarioModificacionId = (int?)null,
            FechaActualizacion = (DateTime?)null
        },
        new
        {
            Id = 2,
            NumeroPedido = "PE-002",
            Cliente = "Maria Gonzalez",
            Fecha = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
            Total = 1200.00m,
            Estado = EstadoPedido.EnProceso,
            Eliminado = false,
            UsuarioCreacionId = 1,
            FechaCreacion = new DateTime(2025, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
            UsuarioModificacionId = (int?)null,
            FechaActualizacion = (DateTime?)null
        },
        new
        {
            Id = 3,
            NumeroPedido = "PE-003",
            Cliente = "Carlos Ramirez",
            Fecha = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            Total = 89.90m,
            Estado = EstadoPedido.Completado,
            Eliminado = false,
            UsuarioCreacionId = 2,
            FechaCreacion = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            UsuarioModificacionId = (int?)null,
            FechaActualizacion = (DateTime?)null
        },
        new
        {
            Id = 4,
            NumeroPedido = "PE-004",
            Cliente = "Ana Torres",
            Fecha = new DateTime(2025, 1, 18, 0, 0, 0, DateTimeKind.Unspecified),
            Total = 430.50m,
            Estado = EstadoPedido.Registrado,
            Eliminado = false,
            UsuarioCreacionId = 2,
            FechaCreacion = new DateTime(2025, 1, 18, 0, 0, 0, DateTimeKind.Unspecified),
            UsuarioModificacionId = (int?)null,
            FechaActualizacion = (DateTime?)null
        },
        new
        {
            Id = 5,
            NumeroPedido = "PE-005",
            Cliente = "Luis Fernandez",
            Fecha = new DateTime(2025, 1, 20, 0, 0, 0, DateTimeKind.Unspecified),
            Total = 75.00m,
            Estado = EstadoPedido.Cancelado,
            Eliminado = false,
            UsuarioCreacionId = 1,
            FechaCreacion = new DateTime(2025, 1, 20, 0, 0, 0, DateTimeKind.Unspecified),
            UsuarioModificacionId = (int?)null,
            FechaActualizacion = (DateTime?)null
        }
    };
}
