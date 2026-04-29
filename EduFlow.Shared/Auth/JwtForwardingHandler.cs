using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace EduFlow.Shared.Auth;

/// <summary>
/// DelegatingHandler that forwards the incoming JWT to outgoing HTTP calls.
///
/// THE PROBLEM IT SOLVES:
/// When M8 (ReportingService) calls M3 (AttendanceTracking), M3 has [Authorize]
/// on every endpoint. If M8 doesn't send a JWT, M3 returns 401. So M8 needs to
/// take the JWT from the incoming request and attach it to outgoing requests.
///
/// HOW TO USE:
/// In Program.cs, register this handler and attach it to every HttpClient:
///
///     builder.Services.AddHttpContextAccessor();
///     builder.Services.AddTransient&lt;JwtForwardingHandler&gt;();
///
///     builder.Services.AddHttpClient&lt;StudentClient&gt;(c =&gt;
///     {
///         c.BaseAddress = new Uri(builder.Configuration["ServiceUrls:StudentService"]!);
///     })
///     .AddHttpMessageHandler&lt;JwtForwardingHandler&gt;();   // ← this line
///
/// AFTER THIS:
/// Every outgoing request from any client (StudentClient, AttendanceClient, etc.)
/// automatically gets `Authorization: Bearer &lt;same-jwt-as-incoming&gt;` attached.
/// No manual header-passing in client code.
/// </summary>
public class JwtForwardingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtForwardingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Pull "Authorization: Bearer xxx" from the incoming request, if present.
        // It might not be present in two valid cases:
        //   - The endpoint that triggered this call is [AllowAnonymous]
        //   - This is a background job with no HTTP context
        // In both cases we just don't forward anything.
        var authHeader = _httpContextAccessor.HttpContext?
            .Request.Headers.Authorization.ToString();

        if (!string.IsNullOrWhiteSpace(authHeader)
            && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader["Bearer ".Length..].Trim();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
