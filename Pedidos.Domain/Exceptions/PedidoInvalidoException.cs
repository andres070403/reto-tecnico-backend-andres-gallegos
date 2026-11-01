namespace Pedidos.Domain.Exceptions;

public sealed class PedidoInvalidoException : DomainException
{
    public PedidoInvalidoException(string message) : base(message)
    {
    }
}
