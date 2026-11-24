using DonationHub.Domain.Exceptions;
using DonationHub.Domain.Primitives;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Api.Exceptions;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {

        var problemDetails = MapExceptionToProblemDetails(exception);

        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        await problemDetailsService.WriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });

        return true;
    }

    private ProblemDetails MapExceptionToProblemDetails(Exception exception)
    {
        if(exception is BaseException baseException)
        {
            (int status, string message, string code) = MapException(exception);


            var problemDetails = new ProblemDetails
            {
                Title = message,
                Status = status,
            };
            problemDetails.Extensions["code"] = code;
            return problemDetails;
        }
        else
        {
            return new ProblemDetails()
            {
                Title = "Something went wrong",
                Status = StatusCodes.Status500InternalServerError,
                Detail = exception.Message,
            };
        }
    }

    private (int status, string title, string code) MapException(Exception exception)
    {
        var baseException = (BaseException)exception;
        return baseException.ExceptionType switch
        {
            ExceptionType.Validation => (StatusCodes.Status400BadRequest, baseException.Message, baseException.Code),
            ExceptionType.Conflict => (StatusCodes.Status409Conflict, baseException.Message, baseException.Code),
            ExceptionType.NotFound => (StatusCodes.Status404NotFound, baseException.Message, baseException.Code),
            ExceptionType.UnprocessableEntity => (StatusCodes.Status422UnprocessableEntity, baseException.Message, baseException.Code),
            ExceptionType.Failure => (StatusCodes.Status500InternalServerError, baseException.Message, baseException.Code),
            _=> (500, "Some thing went wrong", "Request Failed")
        };
    }
}
