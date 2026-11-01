using FluentValidation;
using Pedidos.Application.Dtos.Pedidos;
using Pedidos.Application.Exceptions;
using Pedidos.Application.Interfaces;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Enums;
using ValidationException = Pedidos.Application.Exceptions.ValidationException;

namespace Pedidos.Application.Services;

public sealed class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IValidator<CrearPedidoRequest> _crearValidator;
    private readonly IValidator<ActualizarPedidoRequest> _actualizarValidator;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IValidator<CrearPedidoRequest> crearValidator,
        IValidator<ActualizarPedidoRequest> actualizarValidator)
    {
        _pedidoRepository = pedidoRepository;
        _crearValidator = crearValidator;
        _actualizarValidator = actualizarValidator;
    }

    public Task<List<PedidoResponse>> ObtenerActivosAsync(CancellationToken cancellationToken)
        => _pedidoRepository.ObtenerActivosAsync(cancellationToken);

    public async Task<PedidoResponse> ObtenerPorIdAsync(int id, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoRepository.ObtenerDetalleAsync(id, cancellationToken);
        return pedido ?? throw new NotFoundException($"No se encontró el pedido con id {id}.");
    }

    public async Task<PedidoResponse> CrearAsync(CrearPedidoRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _crearValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (await _pedidoRepository.ExisteNumeroPedidoAsync(request.NumeroPedido, idExcluir: null, cancellationToken))
            throw new ConflictException($"Ya existe un pedido con el número '{request.NumeroPedido}'.");

        var estado = ParsearEstado(request.Estado);
        var pedido = Pedido.Crear(request.NumeroPedido, request.Cliente, request.Fecha, request.Total, estado);

        await _pedidoRepository.AgregarAsync(pedido, cancellationToken);
        await _pedidoRepository.GuardarCambiosAsync(cancellationToken);

        return await _pedidoRepository.ObtenerDetalleAsync(pedido.Id, cancellationToken)
               ?? throw new InvalidOperationException("El pedido recién creado no pudo recuperarse.");
    }

    public async Task<PedidoResponse> ActualizarAsync(int id, ActualizarPedidoRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _actualizarValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var pedido = await _pedidoRepository.ObtenerParaEdicionAsync(id, cancellationToken)
                     ?? throw new NotFoundException($"No se encontró el pedido con id {id}.");

        var estado = ParsearEstado(request.Estado);
        pedido.Actualizar(request.Cliente, request.Fecha, request.Total, estado);

        await _pedidoRepository.GuardarCambiosAsync(cancellationToken);

        return await _pedidoRepository.ObtenerDetalleAsync(id, cancellationToken)
               ?? throw new InvalidOperationException("El pedido recién actualizado no pudo recuperarse.");
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoRepository.ObtenerParaEdicionAsync(id, cancellationToken)
                     ?? throw new NotFoundException($"No se encontró el pedido con id {id}.");

        _pedidoRepository.Eliminar(pedido);
        await _pedidoRepository.GuardarCambiosAsync(cancellationToken);
    }

    private static EstadoPedido ParsearEstado(string estado)
    {
        if (!Enum.TryParse<EstadoPedido>(estado, ignoreCase: true, out var resultado))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                [nameof(CrearPedidoRequest.Estado)] = [$"El estado '{estado}' no es válido."]
            });
        }

        return resultado;
    }
}
