using Microsoft.EntityFrameworkCore;
using ReportingService.Data;
using ReportingService.Models;
using ReportingService.Repositories.Interfaces;

namespace ReportingService.Repositories.Implementations
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await _context.Reports
                .OrderByDescending(r => r.GeneratedAt)
                .ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports.FindAsync(id);
        }

        public async Task<IEnumerable<Report>> GetByScopeAsync(string scope)
        {
            return await _context.Reports
                .Where(r => r.Scope == scope)
                .OrderByDescending(r => r.GeneratedAt)
                .ToListAsync();
        }

        public async Task<Report> CreateAsync(Report report)
        {
            report.GeneratedAt = DateTime.UtcNow;
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task UpdateAsync(Report report)
        {
            _context.Entry(report).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report != null)
            {
                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Reports.AnyAsync(r => r.ReportID == id);
        }
    }
}