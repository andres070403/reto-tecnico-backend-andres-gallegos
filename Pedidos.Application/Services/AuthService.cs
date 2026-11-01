using FluentValidation;
using Microsoft.Extensions.Logging;
using Pedidos.Application.Dtos.Auth;
using Pedidos.Application.Exceptions;
using Pedidos.Application.Interfaces;
using ValidationException = Pedidos.Application.Exceptions.ValidationException;

namespace Pedidos.Application.Services;

public sealed class AuthService : IAuthService
{
    private const string MensajeCredencialesInvalidas = "Email o contraseña incorrectos.";

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IValidator<LoginRequest> _validator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IValidator<LoginRequest> validator,
        ILogger<AuthService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _validator = validator;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var usuario = await _usuarioRepository.ObtenerPorEmailActivoAsync(request.Email, cancellationToken);
        if (usuario is null)
        {
            _logger.LogWarning("Intento de login fallido para el email {Email}: usuario no existe o está inactivo.", request.Email);
            throw new UnauthorizedException(MensajeCredencialesInvalidas);
        }

        if (!_passwordHasher.Verificar(request.Password, usuario.PasswordHash))
        {
            _logger.LogWarning("Intento de login fallido para el email {Email}: contraseña incorrecta.", request.Email);
            throw new UnauthorizedException(MensajeCredencialesInvalidas);
        }

        var (token, expiresIn) = _jwtTokenGenerator.GenerarToken(usuario);
        _logger.LogInformation("Login exitoso para el usuario {UsuarioId} ({Email}).", usuario.Id, usuario.Email);

        return new LoginResponse(token, expiresIn);
    }
}
