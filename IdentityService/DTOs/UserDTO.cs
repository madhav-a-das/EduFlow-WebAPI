using System.ComponentModel.DataAnnotations;

namespace IdentityService.DTOs
{
    // ── UserDTO ──
    // Returned by GET /api/user and GET /api/user/{id}
    // Field names match the requirement entity: UserID, Name, Role, Email, Phone, Status, CreatedAt, UpdatedAt
    // PasswordHash is intentionally excluded — never exposed via API
    public class UserDTO
    {
        public int UserID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // ── UpdateUserDTO ──
    // PUT /api/user/{id} — only editable fields
    // UserID, CreatedAt are server-managed and cannot be changed by the caller
    public class UpdateUserDTO
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        // Active | Inactive | Suspended
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;
    }

    // ── AssignRoleDTO ──
    // POST /api/user/assign-role (Admin only)
    public class AssignRoleDTO
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;
    }
}