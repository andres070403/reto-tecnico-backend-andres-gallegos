namespace Pedidos.Application.Dtos.Pedidos;

public sealed record PedidoResponse(
    int Id,
    string NumeroPedido,
    string Cliente,
    DateTime Fecha,
    decimal Total,
    string Estado,
    string CreadoPor,
    DateTime FechaCreacion,
    string? ModificadoPor,
    DateTime? FechaActualizacion);
