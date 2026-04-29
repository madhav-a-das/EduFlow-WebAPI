using ReportingService.Models;

namespace ReportingService.Services.Interfaces
{
    public interface IReportService
    {
        // CRUD
        Task<IEnumerable<Report>> GetAllAsync();
        Task<Report?> GetByIdAsync(int id);
        Task<IEnumerable<Report>> GetByScopeAsync(string scope);
        Task<Report> CreateAsync(Report report);
        Task UpdateAsync(Report report);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Dashboards
        Task<object> GetEnrollmentDashboardAsync();
        Task<object> GetAcademicDashboardAsync();
        Task<object> GetFinanceDashboardAsync();
    }
}