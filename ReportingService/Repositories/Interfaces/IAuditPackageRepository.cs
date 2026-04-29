using ReportingService.Models;

namespace ReportingService.Repositories.Interfaces
{
    public interface IAuditPackageRepository
    {
        Task<IEnumerable<AuditPackage>> GetAllAsync();
        Task<AuditPackage?> GetByIdAsync(int id);
        Task<IEnumerable<AuditPackage>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task<AuditPackage> CreateAsync(AuditPackage package);
        Task UpdateAsync(AuditPackage package);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}