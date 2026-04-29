using System.Net;

namespace EduFlow.Shared.Errors;

/// <summary>
/// Throw when authentication fails for a domain reason — wrong password,
/// expired session, locked account, etc.
///
/// Note: Most 401s in EduFlow come AUTOMATICALLY from the JWT bearer
/// middleware (when the [Authorize] attribute rejects a missing/bad token).
/// You only throw this manually for domain checks the framework can't do.
///
/// Examples:
/// <code>
/// // Wrong password — login service knows credentials are bad
/// if (!BCrypt.Verify(dto.Password, user.PasswordHash))
///     throw new UnauthorizedException("INVALID_CREDENTIALS",
///         "Email or password is incorrect.");
///
/// // Account locked
/// if (user.Status == "Locked")
///     throw new UnauthorizedException("ACCOUNT_LOCKED",
///         "This account has been locked. Contact your administrator.");
/// </code>
/// </summary>
public class UnauthorizedException : EduFlowException
{
    public UnauthorizedException(string errorCode, string detail)
        : base(HttpStatusCode.Unauthorized, errorCode, "Authentication failed", detail)
    {
    }
}
