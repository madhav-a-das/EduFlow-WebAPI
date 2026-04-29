using ReportingService.Models;

namespace ReportingService.Repositories.Interfaces
{
    public interface IKPIRepository
    {
        Task<IEnumerable<KPI>> GetAllAsync();
        Task<KPI?> GetByIdAsync(int id);
        Task<IEnumerable<KPI>> GetByPeriodAsync(string period);
        Task<KPI> CreateAsync(KPI kpi);
        Task UpdateAsync(KPI kpi);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}