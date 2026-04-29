using System.Security.Claims;

namespace EduFlow.Shared.Auth;

/// <summary>
/// Extension methods on ClaimsPrincipal for reading the current user.
///
/// THE PROBLEM IT SOLVES:
/// M3's AttendanceController had `recordedBy: 1` hardcoded. That means
/// EVERY attendance record looks like it was recorded by UserID 1, no matter
/// who the actual faculty member was. Audit logs are useless.
///
/// HOW TO USE (in any controller):
///     [HttpPost]
///     [Authorize(Roles = "Faculty")]
///     public IActionResult Record(CreateAttendanceDto dto)
///     {
///         var userId = User.GetUserId();        // pulled from JWT claim
///         var role   = User.GetRole();
///         _service.RecordAttendance(dto, userId);
///         return Ok();
///     }
///
/// Inside any service that has IHttpContextAccessor injected:
///     var userId = _httpContext.HttpContext?.User.GetUserId() ?? 0;
/// </summary>
public static class CurrentUserExtensions
{
    /// <summary>
    /// Returns the UserID from the JWT 'NameIdentifier' claim.
    /// Returns 0 if no claim is present (anonymous request).
    /// </summary>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var idClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out var id) ? id : 0;
    }

    /// <summary>
    /// Returns the role string from the JWT 'Role' claim.
    /// Returns empty string if missing.
    /// </summary>
    public static string GetRole(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    /// <summary>
    /// Returns the email from the JWT 'Email' claim.
    /// Returns empty string if missing.
    /// </summary>
    public static string GetEmail(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    /// <summary>
    /// True if the JWT carries an authenticated identity.
    /// </summary>
    public static bool IsAuthenticated(this ClaimsPrincipal user)
        => user.Identity?.IsAuthenticated == true;
}
