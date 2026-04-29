using Microsoft.EntityFrameworkCore;
using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Repositories.Interfaces;

namespace IdentityService.Repositories.Implementations
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userID)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.UserID == userID)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByResourceTypeAsync(string resourceType)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.ResourceType == resourceType)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        // Audit logs are append-only — no Update or Delete methods
        public async Task AddAsync(AuditLog log)
        {
            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}