using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AspDotnetBoilerplate.src.Shared.Exceptions.Handlers;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        
        // Logging
        logger.LogWarning("Unhandled Exception: {Errors}", exception.Message);

        // Status Code definition
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        // Problem Details Service
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error Ocurred",
                Instance = httpContext.Request.Path,
                Detail = exception.Message
            },
            
        });
    }


}