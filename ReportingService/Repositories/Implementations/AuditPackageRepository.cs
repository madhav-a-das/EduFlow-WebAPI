using Microsoft.EntityFrameworkCore;
using ReportingService.Data;
using ReportingService.Models;
using ReportingService.Repositories.Interfaces;

namespace ReportingService.Repositories.Implementations
{
    public class AuditPackageRepository : IAuditPackageRepository
    {
        private readonly AppDbContext _context;

        public AuditPackageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditPackage>> GetAllAsync()
        {
            return await _context.AuditPackages
                .OrderByDescending(a => a.GeneratedAt)
                .ToListAsync();
        }

        public async Task<AuditPackage?> GetByIdAsync(int id)
        {
            return await _context.AuditPackages.FindAsync(id);
        }

        public async Task<IEnumerable<AuditPackage>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.AuditPackages
                .Where(a => a.PeriodStart >= start && a.PeriodEnd <= end)
                .OrderByDescending(a => a.GeneratedAt)
                .ToListAsync();
        }

        public async Task<AuditPackage> CreateAsync(AuditPackage package)
        {
            package.GeneratedAt = DateTime.UtcNow;
            _context.AuditPackages.Add(package);
            await _context.SaveChangesAsync();
            return package;
        }

        public async Task UpdateAsync(AuditPackage package)
        {
            _context.Entry(package).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var package = await _context.AuditPackages.FindAsync(id);
            if (package != null)
            {
                _context.AuditPackages.Remove(package);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.AuditPackages.AnyAsync(a => a.PackageID == id);
        }
    }
}