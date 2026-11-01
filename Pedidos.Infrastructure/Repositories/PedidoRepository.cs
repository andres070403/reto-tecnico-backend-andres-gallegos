using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pedidos.Application.Dtos.Pedidos;
using Pedidos.Application.Exceptions;
using Pedidos.Application.Interfaces;
using Pedidos.Domain.Entities;
using Pedidos.Domain.Enums;
using Pedidos.Infrastructure.Persistence;

namespace Pedidos.Infrastructure.Repositories;

public sealed class PedidoRepository : IPedidoRepository
{
    private const int SqlErrorUniqueIndexViolation = 2601;
    private const int SqlErrorUniqueConstraintViolation = 2627;

    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    private static readonly Expression<Func<Pedido, PedidoProyeccion>> Proyeccion = p => new PedidoProyeccion(
        p.Id,
        p.NumeroPedido,
        p.Cliente,
        p.Fecha,
        p.Total,
        p.Estado,
        p.UsuarioCreacion!.Nombre,
        p.FechaCreacion,
        p.UsuarioModificacion != null ? p.UsuarioModificacion.Nombre : null,
        p.FechaActualizacion);

    public async Task<List<PedidoResponse>> ObtenerActivosAsync(CancellationToken cancellationToken)
    {
        var proyecciones = await _context.Pedidos
            .AsNoTracking()
            .OrderByDescending(p => p.Fecha)
            .Select(Proyeccion)
            .ToListAsync(cancellationToken);

        return proyecciones.Select(AResponse).ToList();
    }

    public async Task<PedidoResponse?> ObtenerDetalleAsync(int id, CancellationToken cancellationToken)
    {
        var proyeccion = await _context.Pedidos
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(Proyeccion)
            .FirstOrDefaultAsync(cancellationToken);

        return proyeccion is null ? null : AResponse(proyeccion);
    }

    public Task<Pedido?> ObtenerParaEdicionAsync(int id, CancellationToken cancellationToken)
        => _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> ExisteNumeroPedidoAsync(string numeroPedido, int? idExcluir, CancellationToken cancellationToken)
    {
        var query = _context.Pedidos.IgnoreQueryFilters().Where(p => p.NumeroPedido == numeroPedido);

        if (idExcluir.HasValue)
            query = query.Where(p => p.Id != idExcluir.Value);

        return query.AnyAsync(cancellationToken);
    }

    public async Task AgregarAsync(Pedido pedido, CancellationToken cancellationToken)
        => await _context.Pedidos.AddAsync(pedido, cancellationToken);

    public void Eliminar(Pedido pedido)
        => _context.Pedidos.Remove(pedido);

    public async Task GuardarCambiosAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (EsViolacionDeUnicidad(ex))
        {
            throw new ConflictException("Ya existe un pedido con ese número.");
        }
    }

    private static bool EsViolacionDeUnicidad(DbUpdateException ex)
        => ex.InnerException is SqlException sqlEx
           && (sqlEx.Number == SqlErrorUniqueIndexViolation || sqlEx.Number == SqlErrorUniqueConstraintViolation);

    private static PedidoResponse AResponse(PedidoProyeccion p) => new(
        p.Id,
        p.NumeroPedido,
        p.Cliente,
        p.Fecha,
        p.Total,
        p.Estado.ToString(),
        p.CreadoPor,
        p.FechaCreacion,
        p.ModificadoPor,
        p.FechaActualizacion);

    private sealed record PedidoProyeccion(
        int Id,
        string NumeroPedido,
        string Cliente,
        DateTime Fecha,
        decimal Total,
        EstadoPedido Estado,
        string CreadoPor,
        DateTime FechaCreacion,
        string? ModificadoPor,
        DateTime? FechaActualizacion);
}
