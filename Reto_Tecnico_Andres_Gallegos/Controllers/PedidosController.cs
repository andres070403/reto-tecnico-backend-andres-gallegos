using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pedidos.Application.Dtos.Pedidos;
using Pedidos.Application.Interfaces;
using Pedidos.Domain.Constants;

namespace Reto_Tecnico_Andres_Gallegos.Controllers;

[ApiController]
[Route("api/pedidos")]
[Authorize]
public sealed class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PedidoResponse>>> ObtenerActivos(CancellationToken cancellationToken)
    {
        var pedidos = await _pedidoService.ObtenerActivosAsync(cancellationToken);
        return Ok(pedidos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PedidoResponse>> ObtenerPorId(int id, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoService.ObtenerPorIdAsync(id, cancellationToken);
        return Ok(pedido);
    }

    [HttpPost]
    public async Task<ActionResult<PedidoResponse>> Crear([FromBody] CrearPedidoRequest request, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoService.CrearAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = pedido.Id }, pedido);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PedidoResponse>> Actualizar(int id, [FromBody] ActualizarPedidoRequest request, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoService.ActualizarAsync(id, request, cancellationToken);
        return Ok(pedido);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        await _pedidoService.EliminarAsync(id, cancellationToken);
        return NoContent();
    }
}
