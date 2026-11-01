namespace Pedidos.Application.Interfaces;

public interface IPasswordHasher
{
    bool Verificar(string password, string hash);
}
