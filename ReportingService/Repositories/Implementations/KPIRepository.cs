using Microsoft.EntityFrameworkCore;
using ReportingService.Data;
using ReportingService.Models;
using ReportingService.Repositories.Interfaces;

namespace ReportingService.Repositories.Implementations
{
    public class KPIRepository : IKPIRepository
    {
        private readonly AppDbContext _context;

        public KPIRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KPI>> GetAllAsync()
        {
            return await _context.KPIs.ToListAsync();
        }

        public async Task<KPI?> GetByIdAsync(int id)
        {
            return await _context.KPIs.FindAsync(id);
        }

        public async Task<IEnumerable<KPI>> GetByPeriodAsync(string period)
        {
            return await _context.KPIs
                .Where(k => k.ReportingPeriod == period)
                .ToListAsync();
        }

        public async Task<KPI> CreateAsync(KPI kpi)
        {
            _context.KPIs.Add(kpi);
            await _context.SaveChangesAsync();
            return kpi;
        }

        public async Task UpdateAsync(KPI kpi)
        {
            _context.Entry(kpi).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var kpi = await _context.KPIs.FindAsync(id);
            if (kpi != null)
            {
                _context.KPIs.Remove(kpi);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.KPIs.AnyAsync(k => k.KPIID == id);
        }
    }
}