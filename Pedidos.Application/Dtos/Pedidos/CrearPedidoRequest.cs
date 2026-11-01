namespace Pedidos.Application.Dtos.Pedidos;

public sealed record CrearPedidoRequest(
    string NumeroPedido,
    string Cliente,
    DateTime Fecha,
    decimal Total,
    string Estado);
