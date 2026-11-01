using Pedidos.Domain.Entities;

namespace Pedidos.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorEmailActivoAsync(string email, CancellationToken cancellationToken);
}
