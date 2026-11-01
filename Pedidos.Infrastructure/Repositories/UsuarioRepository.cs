using Microsoft.EntityFrameworkCore;
using Pedidos.Application.Interfaces;
using Pedidos.Domain.Entities;
using Pedidos.Infrastructure.Persistence;

namespace Pedidos.Infrastructure.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Usuario?> ObtenerPorEmailActivoAsync(string email, CancellationToken cancellationToken)
        => _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.Activo, cancellationToken);
}
