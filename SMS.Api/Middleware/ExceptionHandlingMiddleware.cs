using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace SMS.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private const string UniqueViolationSqlState = "23505";

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
        catch (Exception exception)
        {
            await HandleAsync(context, exception);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = Translate(exception);

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception processing {Method} {Path}.", context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "Request {Method} {Path} failed with {StatusCode}.", context.Request.Method, context.Request.Path, statusCode);
        }

        if (context.Response.HasStarted)
        {
            return;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails, context.RequestAborted);
    }

    private static (int StatusCode, string Title, string Detail) Translate(Exception exception)
    {
        return exception switch
        {
            ArgumentException argumentException =>
                (StatusCodes.Status400BadRequest, "Invalid request.", argumentException.Message),

            DbUpdateException dbUpdateException when IsUniqueViolation(dbUpdateException) =>
                (StatusCodes.Status409Conflict, "Conflict.", "A record with the same unique value already exists."),

            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.",
                "The request could not be completed. Please contact support if the problem persists.")
        };
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException { SqlState: UniqueViolationSqlState };
    }
}
