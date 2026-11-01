using Pedidos.Application.Dtos.Pedidos;
using Pedidos.Domain.Entities;

namespace Pedidos.Application.Interfaces;

public interface IPedidoRepository
{
    Task<List<PedidoResponse>> ObtenerActivosAsync(CancellationToken cancellationToken);

    Task<PedidoResponse?> ObtenerDetalleAsync(int id, CancellationToken cancellationToken);

    Task<Pedido?> ObtenerParaEdicionAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExisteNumeroPedidoAsync(string numeroPedido, int? idExcluir, CancellationToken cancellationToken);

    Task AgregarAsync(Pedido pedido, CancellationToken cancellationToken);

    void Eliminar(Pedido pedido);

    Task GuardarCambiosAsync(CancellationToken cancellationToken);
}
