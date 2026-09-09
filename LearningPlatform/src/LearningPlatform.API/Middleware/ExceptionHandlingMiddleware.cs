using System.Net;
using System.Text.Json;
using LearningPlatform.Application.Exceptions;

namespace LearningPlatform.API.Middleware;

public class ExceptionHandlingMiddleware
{
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, exception.Message),
            BadRequestException => (HttpStatusCode.BadRequest, exception.Message),
            ForbiddenException => (HttpStatusCode.Forbidden, exception.Message),
            ConflictException => (HttpStatusCode.Conflict, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "Dogodila se neočekivana greška na serveru.")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Neobrađena iznimka: {Message}", exception.Message);
        else
            _logger.LogWarning("{ExceptionType}: {Message}", exception.GetType().Name, exception.Message);

        var response = new
        {
            statusCode = (int)statusCode,
            message,
            traceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
