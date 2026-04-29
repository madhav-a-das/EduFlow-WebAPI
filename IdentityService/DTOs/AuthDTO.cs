using System.ComponentModel.DataAnnotations;

namespace IdentityService.DTOs
{
    // ── RegisterDTO ──
    // POST /api/auth/register
    public class RegisterDTO
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        // Student | Faculty | Registrar | DepartmentHead |
        // FinanceOfficer | ComplianceOfficer | Administrator
        public string Role { get; set; } = "Student";
    }

    // ── LoginDTO ──
    // POST /api/auth/login
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // ── TokenResponseDTO ──
    // Returned after successful register or login
    public class TokenResponseDTO
    {
        public string  Token     { get; set; } = string.Empty;
        public int     UserID    { get; set; }
        public string  Name      { get; set; } = string.Empty;
        public string  Email     { get; set; } = string.Empty;
        public string  Role      { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    // ── ChangePasswordDTO ──
    // POST /api/auth/change-password (session management feature)
    public class ChangePasswordDTO
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}