using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AspDotnetBoilerplate.src.Shared.Exceptions.Handlers;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        
        // Logging
        logger.LogWarning("Unhandled Exception: {Errors}", exception.Message);

        // Status Code definition
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        // Problem Details definition
        var problem = new ProblemDetails
        {
            Type = exception.GetType().Name,
            Status = StatusCodes.Status500InternalServerError,
            Title = "Unhandled Exception Ocurred",
            Instance = httpContext.Request.Path,
            Detail = exception.Message
        };

        // return to response
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }


}