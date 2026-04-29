using ReportingService.Models;
using ReportingService.Repositories.Interfaces;
using ReportingService.Services.Interfaces;

namespace ReportingService.Services.Implementations
{
    public class AuditPackageService : IAuditPackageService
    {
        private readonly IAuditPackageRepository _auditPackageRepo;

        public AuditPackageService(IAuditPackageRepository auditPackageRepo)
        {
            _auditPackageRepo = auditPackageRepo;
        }

        public async Task<IEnumerable<AuditPackage>> GetAllAsync()
        {
            return await _auditPackageRepo.GetAllAsync();
        }

        public async Task<AuditPackage?> GetByIdAsync(int id)
        {
            return await _auditPackageRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AuditPackage>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _auditPackageRepo.GetByDateRangeAsync(start, end);
        }

        public async Task<AuditPackage> CreateAsync(AuditPackage package)
        {
            return await _auditPackageRepo.CreateAsync(package);
        }

        public async Task UpdateAsync(AuditPackage package)
        {
            await _auditPackageRepo.UpdateAsync(package);
        }

        public async Task DeleteAsync(int id)
        {
            await _auditPackageRepo.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _auditPackageRepo.ExistsAsync(id);
        }
    }
}