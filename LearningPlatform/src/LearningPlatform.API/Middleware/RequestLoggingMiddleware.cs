using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LearningPlatform.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

            _logger.LogInformation("HTTP Request: {Method} {Path} received from IP: {IpAddress}", method, path, ipAddress);

            await _next(context);
        }
    }
}