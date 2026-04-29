using ReportingService.Models;
using ReportingService.Repositories.Interfaces;
using ReportingService.Services.Interfaces;

namespace ReportingService.Services.Implementations
{
    public class KPIService : IKPIService
    {
        private readonly IKPIRepository _kpiRepo;

        public KPIService(IKPIRepository kpiRepo)
        {
            _kpiRepo = kpiRepo;
        }

        public async Task<IEnumerable<KPI>> GetAllAsync()
        {
            return await _kpiRepo.GetAllAsync();
        }

        public async Task<KPI?> GetByIdAsync(int id)
        {
            return await _kpiRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<KPI>> GetByPeriodAsync(string period)
        {
            return await _kpiRepo.GetByPeriodAsync(period);
        }

        public async Task<KPI> CreateAsync(KPI kpi)
        {
            return await _kpiRepo.CreateAsync(kpi);
        }

        public async Task UpdateAsync(KPI kpi)
        {
            await _kpiRepo.UpdateAsync(kpi);
        }

        public async Task DeleteAsync(int id)
        {
            await _kpiRepo.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _kpiRepo.ExistsAsync(id);
        }
    }
}