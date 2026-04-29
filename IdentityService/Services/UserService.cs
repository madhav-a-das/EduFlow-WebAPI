using IdentityService.DTOs;
using IdentityService.Models;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Interfaces;

namespace IdentityService.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IAuditLogRepository _auditRepo;
        private readonly ILogger<UserService> _logger;

        private static readonly string[] AllowedRoles =
        {
            "Student", "Faculty", "Registrar",
            "DepartmentHead", "FinanceOfficer",
            "ComplianceOfficer", "Administrator"
        };

        public UserService(
            IUserRepository userRepo,
            IAuditLogRepository auditRepo,
            ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _auditRepo = auditRepo;
            _logger = logger;
        }

        // ── User CRUD ──

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDTO?> GetUserByIdAsync(int userID)
        {
            var user = await _userRepo.GetByIdAsync(userID);
            return user == null ? null : MapToDto(user);
        }

        public async Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(string role)
        {
            var users = await _userRepo.GetByRoleAsync(role);
            return users.Select(MapToDto);
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(int userID, UpdateUserDTO dto)
        {
            var user = await _userRepo.GetByIdAsync(userID);
            if (user == null) return (false, "User not found.");

            user.Name = dto.Name;
            user.Phone = dto.Phone;
            user.Status = dto.Status;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);

            _logger.LogInformation("User {UserID} updated", userID);
            return (true, "User updated successfully.");
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int userID)
        {
            var user = await _userRepo.GetByIdAsync(userID);
            if (user == null) return (false, "User not found.");

            // Soft delete — Status = Inactive, row is never physically removed
            // This preserves the audit trail as required by the compliance specification
            user.Status = "Inactive";
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);

            _logger.LogInformation("User {UserID} soft-deleted", userID);
            return (true, "User deactivated successfully.");
        }

        public async Task<(bool Success, string Message)> AssignRoleAsync(AssignRoleDTO dto)
        {
            if (!AllowedRoles.Contains(dto.Role))
                return (false, $"Role must be one of: {string.Join(", ", AllowedRoles)}");

            var user = await _userRepo.GetByIdAsync(dto.UserID);
            if (user == null) return (false, "User not found.");

            var previousRole = user.Role;
            user.Role = dto.Role;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);

            _logger.LogInformation("Role changed for UserID {UserID}: {Old} → {New}",
                dto.UserID, previousRole, dto.Role);

            return (true, $"Role '{dto.Role}' assigned to user '{user.Name}' successfully.");
        }

        // ── Audit log operations ──

        public async Task<IEnumerable<AuditLogDTO>> GetAllLogsAsync()
        {
            var logs = await _auditRepo.GetAllAsync();
            return logs.Select(MapLogToDto);
        }

        public async Task<IEnumerable<AuditLogDTO>> GetLogsByUserAsync(int userID)
        {
            var logs = await _auditRepo.GetByUserIdAsync(userID);
            return logs.Select(MapLogToDto);
        }

        public async Task<IEnumerable<AuditLogDTO>> GetLogsByResourceAsync(string resourceType)
        {
            var logs = await _auditRepo.GetByResourceTypeAsync(resourceType);
            return logs.Select(MapLogToDto);
        }

        // Called internally after every significant action — keeps the audit trail complete
        public async Task LogActionAsync(int userID, string action, string resourceType,
                                         int? resourceID = null, string? details = null)
        {
            await _auditRepo.AddAsync(new AuditLog
            {
                UserID = userID,
                Action = action,
                ResourceType = resourceType,
                ResourceID = resourceID,
                Details = details,
                Timestamp = DateTime.UtcNow
            });
        }

        // ── Private helpers ──

        // Maps User model → UserDTO (PasswordHash is intentionally excluded)
        private static UserDTO MapToDto(User u) => new UserDTO
        {
            UserID = u.UserID,
            Name = u.Name,
            Role = u.Role,
            Email = u.Email,
            Phone = u.Phone,
            Status = u.Status,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        };

        private static AuditLogDTO MapLogToDto(AuditLog a) => new AuditLogDTO
        {
            AuditID = a.AuditID,
            UserID = a.UserID,
            UserName = a.User?.Name ?? "Unknown",
            Action = a.Action,
            ResourceType = a.ResourceType,
            ResourceID = a.ResourceID,
            Details = a.Details,
            Timestamp = a.Timestamp
        };
    }
}