using IdentityService.DTOs;

namespace IdentityService.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int userID);
        Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(string role);
        Task<(bool Success, string Message)> UpdateUserAsync(int userID, UpdateUserDTO dto);
        Task<(bool Success, string Message)> DeleteUserAsync(int userID);
        Task<(bool Success, string Message)> AssignRoleAsync(AssignRoleDTO dto);

        // Audit log operations
        Task<IEnumerable<AuditLogDTO>> GetAllLogsAsync();
        Task<IEnumerable<AuditLogDTO>> GetLogsByUserAsync(int userID);
        Task<IEnumerable<AuditLogDTO>> GetLogsByResourceAsync(string resourceType);
        Task LogActionAsync(int userID, string action, string resourceType,
                            int? resourceID = null, string? details = null);
    }
}