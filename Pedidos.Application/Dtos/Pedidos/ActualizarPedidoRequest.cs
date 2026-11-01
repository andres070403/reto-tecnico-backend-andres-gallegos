namespace Pedidos.Application.Dtos.Pedidos;

public sealed record ActualizarPedidoRequest(
    string Cliente,
    DateTime Fecha,
    decimal Total,
    string Estado);
