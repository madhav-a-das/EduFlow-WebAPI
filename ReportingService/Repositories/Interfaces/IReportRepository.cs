using ReportingService.Models;

namespace ReportingService.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<Report>> GetAllAsync();
        Task<Report?> GetByIdAsync(int id);
        Task<IEnumerable<Report>> GetByScopeAsync(string scope);
        Task<Report> CreateAsync(Report report);
        Task UpdateAsync(Report report);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}