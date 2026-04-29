using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EduFlow.Shared.Middleware;

/// <summary>
/// Logs every HTTP request with method, path, status code, and duration.
/// Picks log level based on outcome:
///   - 2xx/3xx → Information
///   - 4xx     → Warning
///   - 5xx     → Error
///
/// Register ONCE, after ExceptionMiddleware:
///     app.UseMiddleware&lt;ExceptionMiddleware&gt;();
///     app.UseMiddleware&lt;RequestLoggingMiddleware&gt;();
///
/// IMPORTANT: M3's original Program.cs registered this twice. That doubled
/// every log line. Each middleware should appear exactly once.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        await _next(context);
        stopwatch.Stop();

        var elapsedMs  = stopwatch.ElapsedMilliseconds;
        var method     = context.Request.Method;
        var path       = context.Request.Path.Value;
        var statusCode = context.Response.StatusCode;

        if (statusCode >= 500)
        {
            _logger.LogError(
                "HTTP {Method} {Path} → {StatusCode} in {ElapsedMs}ms",
                method, path, statusCode, elapsedMs);
        }
        else if (statusCode >= 400)
        {
            _logger.LogWarning(
                "HTTP {Method} {Path} → {StatusCode} in {ElapsedMs}ms",
                method, path, statusCode, elapsedMs);
        }
        else
        {
            _logger.LogInformation(
                "HTTP {Method} {Path} → {StatusCode} in {ElapsedMs}ms",
                method, path, statusCode, elapsedMs);
        }
    }
}
