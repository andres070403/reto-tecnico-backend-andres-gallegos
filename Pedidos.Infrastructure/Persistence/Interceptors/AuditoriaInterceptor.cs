using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pedidos.Application.Interfaces;
using Pedidos.Domain.Entities;

namespace Pedidos.Infrastructure.Persistence.Interceptors;

public sealed class AuditoriaInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditoriaInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AplicarAuditoria(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AplicarAuditoria(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AplicarAuditoria(DbContext? context)
    {
        if (context is null)
            return;

        var ahora = DateTime.Now;
        var usuarioActualId = _currentUserService.UsuarioId;

        foreach (var entry in context.ChangeTracker.Entries<Pedido>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(p => p.FechaCreacion).CurrentValue = ahora;
                    entry.Property(p => p.UsuarioCreacionId).CurrentValue = ObtenerUsuarioActualId(usuarioActualId);
                    break;

                case EntityState.Modified:
                    entry.Property(p => p.FechaActualizacion).CurrentValue = ahora;
                    entry.Property(p => p.UsuarioModificacionId).CurrentValue = ObtenerUsuarioActualId(usuarioActualId);
                    break;

                case EntityState.Deleted:
                    entry.Entity.MarcarComoEliminado();
                    entry.Property(p => p.FechaActualizacion).CurrentValue = ahora;
                    entry.Property(p => p.UsuarioModificacionId).CurrentValue = ObtenerUsuarioActualId(usuarioActualId);
                    entry.State = EntityState.Modified;
                    break;
            }
        }
    }

    private static int ObtenerUsuarioActualId(int? usuarioActualId)
        => usuarioActualId
           ?? throw new InvalidOperationException(
               "No se pudo determinar el usuario autenticado para registrar la auditoría del pedido.");
}
