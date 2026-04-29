namespace EduFlow.Shared.Auth;

/// <summary>
/// Strongly-typed JWT configuration loaded from appsettings.json under the
/// "JwtSettings" section.
///
/// CRITICAL: Issuer, Audience, and SecretKey must be IDENTICAL across every
/// EduFlow service. If they diverge, M1 will issue tokens that M3/M8/etc.
/// cannot validate, and every cross-service call will fail with 401.
///
/// In production these values must come from environment variables or a
/// secret manager — never from a committed appsettings file.
/// </summary>
public class JwtSettings
{
    /// <summary>Who issued the token. Always "EduFlow".</summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>Who the token is for. Always "EduFlowClients".</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>HMAC signing key. Minimum 32 chars for HS256.</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>How long a freshly-issued token is valid (in minutes).</summary>
    public int ExpiryMinutes { get; set; } = 60;
}
