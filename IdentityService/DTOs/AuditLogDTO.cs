using System.ComponentModel.DataAnnotations;

namespace IdentityService.DTOs
{
    // ── AuditLogDTO ──
    // Returned by GET /api/auditlog endpoints
    // Field names match the requirement entity: AuditID, UserID, Action, ResourceType, ResourceID, Details, Timestamp
    // UserName is added for convenience — avoids a second API call to look up the name
    public class AuditLogDTO
    {
        public int AuditID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;   // joined from User table
        public string Action { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public int? ResourceID { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // ── CreateAuditLogDTO ──
    // POST /api/auditlog — used by other services to log their own actions
    public class CreateAuditLogDTO
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ResourceType { get; set; } = string.Empty;

        public int? ResourceID { get; set; }
        public string? Details { get; set; }
    }
}