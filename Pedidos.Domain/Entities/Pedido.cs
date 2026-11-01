using Pedidos.Domain.Enums;
using Pedidos.Domain.Exceptions;

namespace Pedidos.Domain.Entities;

public sealed class Pedido
{
    public const int NumeroPedidoMaxLength = 50;
    public const int ClienteMaxLength = 150;

    public int Id { get; private set; }
    public string NumeroPedido { get; private set; } = null!;
    public string Cliente { get; private set; } = null!;
    public DateTime Fecha { get; private set; }
    public decimal Total { get; private set; }
    public EstadoPedido Estado { get; private set; }
    public bool Eliminado { get; private set; }

    public int UsuarioCreacionId { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public int? UsuarioModificacionId { get; private set; }
    public DateTime? FechaActualizacion { get; private set; }

    public Usuario? UsuarioCreacion { get; private set; }
    public Usuario? UsuarioModificacion { get; private set; }

    private Pedido()
    {
    }

    public static Pedido Crear(string numeroPedido, string cliente, DateTime fecha, decimal total, EstadoPedido estado)
    {
        ValidarNumeroPedido(numeroPedido);
        ValidarCliente(cliente);
        ValidarTotal(total);

        return new Pedido
        {
            NumeroPedido = numeroPedido.Trim(),
            Cliente = cliente.Trim(),
            Fecha = fecha,
            Total = total,
            Estado = estado,
            Eliminado = false
        };
    }

    public void Actualizar(string cliente, DateTime fecha, decimal total, EstadoPedido estado)
    {
        ValidarCliente(cliente);
        ValidarTotal(total);

        Cliente = cliente.Trim();
        Fecha = fecha;
        Total = total;
        Estado = estado;
    }

    public void MarcarComoEliminado()
    {
        Eliminado = true;
    }

    private static void ValidarNumeroPedido(string numeroPedido)
    {
        if (string.IsNullOrWhiteSpace(numeroPedido))
            throw new PedidoInvalidoException("El número de pedido es obligatorio.");

        if (numeroPedido.Length > NumeroPedidoMaxLength)
            throw new PedidoInvalidoException($"El número de pedido no puede exceder {NumeroPedidoMaxLength} caracteres.");
    }

    private static void ValidarCliente(string cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente))
            throw new PedidoInvalidoException("El cliente es obligatorio.");

        if (cliente.Length > ClienteMaxLength)
            throw new PedidoInvalidoException($"El cliente no puede exceder {ClienteMaxLength} caracteres.");
    }

    private static void ValidarTotal(decimal total)
    {
        if (total <= 0)
            throw new PedidoInvalidoException("El total del pedido debe ser mayor a cero.");
    }
}
