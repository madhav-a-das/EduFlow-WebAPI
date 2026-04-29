using IdentityService.Models;

namespace IdentityService.Repositories.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<IEnumerable<AuditLog>> GetAllAsync();
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userID);
        Task<IEnumerable<AuditLog>> GetByResourceTypeAsync(string resourceType);
        Task AddAsync(AuditLog log);
    }
}