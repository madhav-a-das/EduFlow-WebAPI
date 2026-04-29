using System.Text;
using System.Text.Json;
using EduFlow.Shared.Errors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EduFlow.Shared.Auth;

/// <summary>
/// One-liner JWT setup that every EduFlow service calls in Program.cs.
///
/// Usage:
/// <code>
/// builder.Services.AddEduFlowJwtAuth(builder.Configuration);
/// </code>
///
/// What this does:
///   1. Reads "JwtSettings" section from appsettings.json
///   2. Configures JwtBearer middleware with the validation rules every
///      service must use (issuer, audience, lifetime, signing key)
///   3. Wires JwtBearerEvents so 401 and 403 responses use the same
///      ApiErrorResponse JSON shape as everything else
///   4. Adds Authorization services so [Authorize] attributes work
///
/// Single source of truth — change JWT validation rules HERE and every
/// service inherits the change.
/// </summary>
public static class JwtAuthenticationExtensions
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy   = JsonNamingPolicy.CamelCase
    };

    public static IServiceCollection AddEduFlowJwtAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("JwtSettings");
        var jwtSettings = jwtSection.Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                "Missing 'JwtSettings' section in appsettings.json. " +
                "Every EduFlow service requires JwtSettings.Issuer, " +
                "JwtSettings.Audience, and JwtSettings.SecretKey.");

        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
        {
            throw new InvalidOperationException(
                "JwtSettings.SecretKey is empty. Set it in appsettings.json " +
                "(dev) or via env var JwtSettings__SecretKey (prod).");
        }

        services.Configure<JwtSettings>(jwtSection);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = jwtSettings.Issuer,
                    ValidAudience            = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

                // Make 401 and 403 responses use the standard EduFlow shape
                // instead of empty bodies / inconsistent shapes.
                options.Events = new JwtBearerEvents
                {
                    OnChallenge       = HandleChallenge,
                    OnForbidden       = HandleForbidden
                };
            });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Fires when JwtBearer middleware decides to issue a 401.
    /// Without this, the framework writes an empty body. We replace it
    /// with a JSON ApiErrorResponse.
    /// </summary>
    private static async Task HandleChallenge(JwtBearerChallengeContext context)
    {
        // Skip the framework's default empty-body 401 response
        context.HandleResponse();

        var detail = context.AuthenticateFailure switch
        {
            null => "A valid JWT token is required to access this resource.",
            { } ex when ex.GetType().Name.Contains("SecurityTokenExpired")
                => "Your session has expired. Please log in again.",
            { } ex when ex.GetType().Name.Contains("SecurityTokenInvalidSignature")
                => "The provided token signature is invalid.",
            _ => "The provided token could not be validated."
        };

        var errorCode = context.AuthenticateFailure switch
        {
            null => "AUTH_REQUIRED",
            { } ex when ex.GetType().Name.Contains("SecurityTokenExpired") => "TOKEN_EXPIRED",
            _ => "TOKEN_INVALID"
        };

        var body = new ApiErrorResponse
        {
            Status    = StatusCodes.Status401Unauthorized,
            Title     = "Authentication required",
            Detail    = detail,
            ErrorCode = errorCode,
            Instance  = context.Request.Path.Value,
            TraceId   = context.HttpContext.TraceIdentifier
        };

        context.Response.StatusCode  = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOpts));
    }

    /// <summary>
    /// Fires when [Authorize(Roles = "...")] rejects an authenticated user.
    /// </summary>
    private static async Task HandleForbidden(ForbiddenContext context)
    {
        var body = new ApiErrorResponse
        {
            Status    = StatusCodes.Status403Forbidden,
            Title     = "Access denied",
            Detail    = "You do not have permission to access this resource.",
            ErrorCode = "INSUFFICIENT_ROLE",
            Instance  = context.Request.Path.Value,
            TraceId   = context.HttpContext.TraceIdentifier
        };

        context.Response.StatusCode  = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOpts));
    }
}
