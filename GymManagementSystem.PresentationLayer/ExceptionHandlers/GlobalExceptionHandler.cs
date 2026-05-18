using System.Diagnostics;
using GymManagementSystem.BusinessLayer.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GymManagementSystem.PresentationLayer.ExceptionHandlers;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IWebHostEnvironment env,
    ITempDataDictionaryFactory tempDataFactory)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken ct)
    {
        var (statusCode, logLevel) = MapException(exception);
        var requestId = Activity.Current?.Id ?? context.TraceIdentifier;

        logger.Log(logLevel, exception,
            "Unhandled exception {ExceptionType} for {Method} {Path} (RequestId: {RequestId})",
            exception.GetType().Name,
            context.Request.Method,
            context.Request.Path,
            requestId);

        if (IsApiRequest(context))
        {
            await WriteJsonResponseAsync(context, exception, statusCode, requestId, ct);
        }
        else
        {
            WriteRedirectResponse(context, exception, statusCode, requestId);
        }

        return true;
    }

    private async Task WriteJsonResponseAsync(
        HttpContext context,
        Exception exception,
        int statusCode,
        string requestId,
        CancellationToken ct)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Title = GetTitleForStatusCode(statusCode),
            Status = statusCode,
            Instance = context.Request.Path,
            Detail = env.IsDevelopment() ? exception.Message : null,
            Extensions =
            {
                ["requestId"] = requestId,
                ["traceId"] = requestId
            }
        };

        if (exception is ValidationException { Details: not null } validationEx)
        {
            problemDetails.Extensions["errors"] = validationEx.Details;
        }

        await context.Response.WriteAsJsonAsync(problemDetails, ct);
    }

    private void WriteRedirectResponse(
        HttpContext context,
        Exception exception,
        int statusCode,
        string requestId)
    {
        var errorMessage = env.IsDevelopment()
            ? exception.Message
            : "An error occurred while processing your request.";

        var tempData = tempDataFactory.GetTempData(context);
        tempData["ErrorMessage"] = errorMessage;
        tempData["StatusCode"] = statusCode.ToString();
        tempData["RequestId"] = requestId;

        if (env.IsDevelopment())
        {
            tempData["ExceptionDetails"] = exception.ToString();
        }

        tempData.Save();

        context.Response.Redirect("/Home/Error");
    }

    private static (int StatusCode, LogLevel LogLevel) MapException(Exception exception)
    {
        return exception switch
        {
            NotFoundException => (404, LogLevel.Warning),
            ValidationException => (400, LogLevel.Warning),
            ConflictException => (409, LogLevel.Warning),
            ForbiddenException => (403, LogLevel.Warning),
            _ => (500, LogLevel.Error)
        };
    }

    private static bool IsApiRequest(HttpContext context)
    {
        return context.Request.Headers.Accept.ToString().Contains("application/json")
            || string.Equals(
                context.Request.Headers.XRequestedWith,
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase);
    }

    private static string GetTitleForStatusCode(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        _ => "Internal Server Error"
    };
}
