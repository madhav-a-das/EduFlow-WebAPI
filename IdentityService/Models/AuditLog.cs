using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditID { get; set; }

        // FK → User.UserID — who performed the action
        public int UserID { get; set; }

        // e.g. LOGIN, LOGOUT, CREATE, UPDATE, DELETE, ASSIGN_ROLE, PASSWORD_RESET
        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        // Which entity type was affected — e.g. "User", "Enrollment", "Course"
        [Required]
        [MaxLength(100)]
        public string ResourceType { get; set; } = string.Empty;

        // The ID of the affected record — nullable (some actions have no specific resource)
        public int? ResourceID { get; set; }

        // Free-text or JSON describing what changed — stored as TEXT in SQL Server
        [Column(TypeName = "text")]
        public string? Details { get; set; }

        // Set once at write time — never updated (append-only requirement)
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation property → User who performed the action
        [ForeignKey("UserID")]
        public User? User { get; set; }
    }
}