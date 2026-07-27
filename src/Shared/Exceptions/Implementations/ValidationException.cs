namespace AspDotnetBoilerplate.src.Shared.Exceptions.Implementations;

public sealed class ValidationException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors) 
    : base("One or more validation errors occurred")
    {
        Errors = errors.ToList();
    }
    
}