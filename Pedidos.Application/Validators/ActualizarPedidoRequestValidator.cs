using FluentValidation;
using Pedidos.Application.Dtos.Pedidos;
using Pedidos.Domain.Entities;

namespace Pedidos.Application.Validators;

public sealed class ActualizarPedidoRequestValidator : AbstractValidator<ActualizarPedidoRequest>
{
    public ActualizarPedidoRequestValidator()
    {
        RuleFor(x => x.Cliente)
            .NotEmpty().WithMessage("El cliente es obligatorio.")
            .MaximumLength(Pedido.ClienteMaxLength)
            .WithMessage($"El cliente no puede exceder {Pedido.ClienteMaxLength} caracteres.");

        RuleFor(x => x.Fecha)
            .NotEqual(default(DateTime)).WithMessage("La fecha es obligatoria.");

        RuleFor(x => x.Total)
            .GreaterThan(0).WithMessage("El total debe ser mayor a cero.");

        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("El estado es obligatorio.")
            .DebeSerEstadoValido();
    }
}
