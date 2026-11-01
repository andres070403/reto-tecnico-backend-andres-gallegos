using Pedidos.Domain.Entities;

namespace Pedidos.Application.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, int ExpiresIn) GenerarToken(Usuario usuario);
}
