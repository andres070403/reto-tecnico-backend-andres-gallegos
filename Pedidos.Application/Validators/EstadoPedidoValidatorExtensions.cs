using FluentValidation;
using Pedidos.Domain.Enums;

namespace Pedidos.Application.Validators;

internal static class EstadoPedidoValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> DebeSerEstadoValido<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(estado => Enum.TryParse<EstadoPedido>(estado, ignoreCase: true, out _))
            .WithMessage($"El estado debe ser uno de: {string.Join(", ", Enum.GetNames<EstadoPedido>())}.");
    }
}
