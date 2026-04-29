using Microsoft.AspNetCore.Mvc;
using ReportingService.Models;
using ReportingService.Services.Interfaces;

namespace ReportingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KPIController : ControllerBase
    {
        private readonly IKPIService _kpiService;

        public KPIController(IKPIService kpiService)
        {
            _kpiService = kpiService;
        }

        // GET: api/KPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KPI>>> GetAllKPIs()
        {
            var kpis = await _kpiService.GetAllAsync();
            return Ok(kpis);
        }

        // GET: api/KPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KPI>> GetKPI(int id)
        {
            var kpi = await _kpiService.GetByIdAsync(id);
            if (kpi == null) return NotFound();
            return Ok(kpi);
        }

        // GET: api/KPI/period/Q1 2025
        [HttpGet("period/{period}")]
        public async Task<ActionResult<IEnumerable<KPI>>> GetKPIsByPeriod(string period)
        {
            var kpis = await _kpiService.GetByPeriodAsync(period);
            return Ok(kpis);
        }

        // POST: api/KPI
        [HttpPost]
        public async Task<ActionResult<KPI>> CreateKPI(KPI kpi)
        {
            var created = await _kpiService.CreateAsync(kpi);
            return CreatedAtAction(nameof(GetKPI), new { id = created.KPIID }, created);
        }

        // PUT: api/KPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKPI(int id, KPI kpi)
        {
            if (id != kpi.KPIID) return BadRequest();
            if (!await _kpiService.ExistsAsync(id)) return NotFound();

            await _kpiService.UpdateAsync(kpi);
            return NoContent();
        }

        // DELETE: api/KPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKPI(int id)
        {
            if (!await _kpiService.ExistsAsync(id)) return NotFound();

            await _kpiService.DeleteAsync(id);
            return NoContent();
        }
    }
}