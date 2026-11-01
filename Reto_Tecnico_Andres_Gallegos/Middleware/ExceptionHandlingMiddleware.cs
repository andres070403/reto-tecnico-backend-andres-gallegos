using Microsoft.AspNetCore.Mvc;
using Pedidos.Application.Exceptions;
using Pedidos.Domain.Exceptions;

namespace Reto_Tecnico_Andres_Gallegos.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await EscribirProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Error de validación", ex.Message, ex.Errors);
        }
        catch (PedidoInvalidoException ex)
        {
            await EscribirProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Error de validación", ex.Message);
        }
        catch (NotFoundException ex)
        {
            await EscribirProblemDetailsAsync(context, StatusCodes.Status404NotFound, "Recurso no encontrado", ex.Message);
        }
        catch (ConflictException ex)
        {
            await EscribirProblemDetailsAsync(context, StatusCodes.Status409Conflict, "Conflicto", ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            await EscribirProblemDetailsAsync(context, StatusCodes.Status401Unauthorized, "No autorizado", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado procesando {Metodo} {Ruta}", context.Request.Method, context.Request.Path);
            await EscribirProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                "Ocurrió un error inesperado. Intente nuevamente más tarde.");
        }
    }

    private static Task EscribirProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail,
        IDictionary<string, string[]>? errores = null)
    {
        if (context.Response.HasStarted)
            return Task.CompletedTask;

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (errores is not null)
            problemDetails.Extensions["errors"] = errores;

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}
