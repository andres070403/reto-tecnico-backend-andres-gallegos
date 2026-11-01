namespace Pedidos.Application.Exceptions;

public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Se encontraron uno o más errores de validación.")
    {
        Errors = errors;
    }

    public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : this(failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray()))
    {
    }
}
