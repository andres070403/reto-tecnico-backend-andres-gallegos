using Pedidos.Application.Dtos.Pedidos;

namespace Pedidos.Application.Interfaces;

public interface IPedidoService
{
    Task<List<PedidoResponse>> ObtenerActivosAsync(CancellationToken cancellationToken);
    Task<PedidoResponse> ObtenerPorIdAsync(int id, CancellationToken cancellationToken);
    Task<PedidoResponse> CrearAsync(CrearPedidoRequest request, CancellationToken cancellationToken);
    Task<PedidoResponse> ActualizarAsync(int id, ActualizarPedidoRequest request, CancellationToken cancellationToken);
    Task EliminarAsync(int id, CancellationToken cancellationToken);
}
