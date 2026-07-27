using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AspDotnetBoilerplate.src.Shared.Exceptions.Handlers;


public class ValidationExceptionHandler(
    ILogger<ValidationExceptionHandler> logger 
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        // Logging
        logger.LogWarning("Validation Failed: {Errors}", validationException.Errors);

        // Status Code definition
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        // Problem Details definition
        var problem = new ValidationProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error Ocurred",
            Instance = httpContext.Request.Path,
        };

        // return to response
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }



}