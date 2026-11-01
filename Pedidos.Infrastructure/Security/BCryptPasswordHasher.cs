using Pedidos.Application.Interfaces;

namespace Pedidos.Infrastructure.Security;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    public bool Verificar(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
