using System.Net;
using System.Text.Json;
using EduFlow.Shared.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EduFlow.Shared.Middleware;

/// <summary>
/// Catches every unhandled exception in the pipeline and converts it into a
/// standard ApiErrorResponse JSON body. This is the single source of truth
/// for what EduFlow's error responses look like.
///
/// Register FIRST in the pipeline:
/// <code>
/// app.UseMiddleware&lt;ExceptionMiddleware&gt;();
/// app.UseAuthentication();
/// app.UseAuthorization();
/// app.MapControllers();
/// </code>
/// If registered after MapControllers, it never catches anything.
///
/// Handles three kinds of exception:
///   1. EduFlowException (typed domain errors) — uses the exception's own
///      status code, error code, title, and detail
///   2. Common framework exceptions (HttpRequestException, TaskCanceledException) —
///      mapped to sensible HTTP codes
///   3. Anything else — 500 INTERNAL_ERROR with detail hidden in production
/// </summary>
public class ExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy   = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        var response = BuildResponse(context, ex);

        // Log at the appropriate level
        if (response.Status >= 500)
        {
            _logger.LogError(ex,
                "Unhandled {StatusCode} on {Method} {Path}: {ErrorCode}",
                response.Status, context.Request.Method, context.Request.Path,
                response.ErrorCode);
        }
        else
        {
            // 4xx are expected business outcomes — don't log full stack
            _logger.LogWarning(
                "{StatusCode} on {Method} {Path}: {ErrorCode} — {Detail}",
                response.Status, context.Request.Method, context.Request.Path,
                response.ErrorCode, response.Detail);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = response.Status;

        var json = JsonSerializer.Serialize(response, JsonOptions);
        await context.Response.WriteAsync(json);
    }

    private ApiErrorResponse BuildResponse(HttpContext context, Exception ex)
    {
        var instance = context.Request.Path.Value;
        var traceId  = context.TraceIdentifier;

        // Case 1 — typed EduFlow domain exception
        if (ex is EduFlowException eduEx)
        {
            var resp = new ApiErrorResponse
            {
                Status    = (int)eduEx.StatusCode,
                Title     = eduEx.Title,
                Detail    = eduEx.Message,
                ErrorCode = eduEx.ErrorCode,
                Instance  = instance,
                TraceId   = traceId
            };

            // Field-level validation errors
            if (eduEx is ValidationException ve && ve.FieldErrors != null)
            {
                resp.Errors = ve.FieldErrors;
            }

            return resp;
        }

        // Case 2 — well-known framework exceptions
        return ex switch
        {
            HttpRequestException => new ApiErrorResponse
            {
                Status    = (int)HttpStatusCode.ServiceUnavailable,
                Title     = "Service unavailable",
                Detail    = "An upstream service is unreachable. Please try again shortly.",
                ErrorCode = "UPSTREAM_UNAVAILABLE",
                Instance  = instance,
                TraceId   = traceId
            },

            TaskCanceledException => new ApiErrorResponse
            {
                Status    = (int)HttpStatusCode.GatewayTimeout,
                Title     = "Request timed out",
                Detail    = "An upstream service did not respond in time.",
                ErrorCode = "UPSTREAM_TIMEOUT",
                Instance  = instance,
                TraceId   = traceId
            },

            UnauthorizedAccessException => new ApiErrorResponse
            {
                Status    = (int)HttpStatusCode.Unauthorized,
                Title     = "Authentication required",
                Detail    = ex.Message,
                ErrorCode = "AUTH_REQUIRED",
                Instance  = instance,
                TraceId   = traceId
            },

            // Case 3 — anything else is a 500 bug
            _ => new ApiErrorResponse
            {
                Status    = (int)HttpStatusCode.InternalServerError,
                Title     = "Internal server error",
                Detail    = _env.IsDevelopment() ? ex.ToString() : null,
                ErrorCode = "INTERNAL_ERROR",
                Instance  = instance,
                TraceId   = traceId
            }
        };
    }
}
