using System.Net;
using System.Text.Json;
using InventoryManagementApi.Exceptions;

namespace InventoryManagementApi.Middleware;

/// <summary>
/// Global exception-handling middleware.
///
/// Catches any unhandled exceptions thrown deeper in the pipeline
/// (controllers, services, EF) and converts them to consistent JSON
/// error responses. Centralizing this avoids try/catch boilerplate
/// in every controller action.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while processing {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // Map known exception types to HTTP status codes.
        var (status, message) = ex switch
        {
            NotFoundException => (HttpStatusCode.NotFound, ex.Message),
            ArgumentException => (HttpStatusCode.BadRequest, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var payload = JsonSerializer.Serialize(new
        {
            timestamp = DateTime.UtcNow,
            status = (int)status,
            message,
            path = context.Request.Path.Value
        });

        return context.Response.WriteAsync(payload);
    }
}
