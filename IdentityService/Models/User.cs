using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Role drives what the user can access:
        // Student, Faculty, Registrar, DepartmentHead,
        // FinanceOfficer, ComplianceOfficer, Administrator
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        // Active | Inactive | Suspended
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Internal field — stores BCrypt hash, never exposed in DTOs or API responses
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Navigation property — one user can have many audit log entries
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}