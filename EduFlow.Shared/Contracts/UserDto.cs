namespace EduFlow.Shared.Contracts;

/// <summary>
/// Public shape of a User as returned by IdentityService (M1) endpoints.
/// PasswordHash is intentionally absent — never crosses a service boundary.
///
/// All services that consume M1's user data deserialize into THIS class,
/// not their own copy. If M1's response shape changes, every service updates
/// in lockstep just by pulling a new EduFlow.Shared.
/// </summary>
public class UserDto
{
    public int UserID { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Student | Faculty | Registrar | DepartmentHead |
    /// FinanceOfficer | ComplianceOfficer | Administrator
    /// </summary>
    public string Role { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    /// <summary>Active | Inactive | Suspended</summary>
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
