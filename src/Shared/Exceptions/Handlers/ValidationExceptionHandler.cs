
using AspDotnetBoilerplate.src.Shared.Exceptions.Implementations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AspDotnetBoilerplate.src.Shared.Exceptions.Handlers;


public sealed class ValidationExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ValidationExceptionHandler> logger 
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Send exception to another handler
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        // Logging
        logger.LogWarning("Validation Failed: {Errors}", validationException.Errors);

        // Status Code definition
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors ocurred",
                Instance = httpContext.Request.Path,
                Detail = exception.Message
            },
            
        };

        context.ProblemDetails.Extensions.Add("errors", validationException.Errors);

        // Problem Details Service
        return await problemDetailsService.TryWriteAsync(context);
    }



}