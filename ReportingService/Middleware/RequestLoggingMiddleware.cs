using System.Diagnostics;

namespace ReportingService.Middleware
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
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            var method = context.Request.Method;
            var path = context.Request.Path;
            var statusCode = context.Response.StatusCode;

            if (statusCode >= 400 && statusCode < 500)
            {
                _logger.LogWarning(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                    method, path, statusCode, elapsedMs);
            }
            else if (statusCode >= 500)
            {
                _logger.LogError(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                    method, path, statusCode, elapsedMs);
            }
            else
            {
                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                    method, path, statusCode, elapsedMs);
            }
        }
    }
}