using Microsoft.AspNetCore.Mvc;
using ReportingService.Models;
using ReportingService.Services.Interfaces;

namespace ReportingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        //  CRUD ENDPOINTS

        // GET: api/Report
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> GetAllReports()
        {
            var reports = await _reportService.GetAllAsync();
            return Ok(reports);
        }

        // GET: api/Report/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Report>> GetReport(int id)
        {
            var report = await _reportService.GetByIdAsync(id);
            if (report == null) return NotFound();
            return Ok(report);
        }

        // GET: api/Report/scope/Academic
        [HttpGet("scope/{scope}")]
        public async Task<ActionResult<IEnumerable<Report>>> GetReportsByScope(string scope)
        {
            var reports = await _reportService.GetByScopeAsync(scope);
            return Ok(reports);
        }

        // POST: api/Report
        [HttpPost]
        public async Task<ActionResult<Report>> CreateReport(Report report)
        {
            var created = await _reportService.CreateAsync(report);
            return CreatedAtAction(nameof(GetReport), new { id = created.ReportID }, created);
        }

        // PUT: api/Report/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(int id, Report report)
        {
            if (id != report.ReportID) return BadRequest();
            if (!await _reportService.ExistsAsync(id)) return NotFound();

            await _reportService.UpdateAsync(report);
            return NoContent();
        }

        // DELETE: api/Report/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            if (!await _reportService.ExistsAsync(id)) return NotFound();

            await _reportService.DeleteAsync(id);
            return NoContent();
        }


        //  DASHBOARD ENDPOINTS (Cross-Service)


        // GET: api/Report/dashboard/enrollment
        [HttpGet("dashboard/enrollment")]
        public async Task<ActionResult<object>> GetEnrollmentDashboard()
        {
            var result = await _reportService.GetEnrollmentDashboardAsync();
            return Ok(result);
        }

        // GET: api/Report/dashboard/academic
        [HttpGet("dashboard/academic")]
        public async Task<ActionResult<object>> GetAcademicDashboard()
        {
            var result = await _reportService.GetAcademicDashboardAsync();
            return Ok(result);
        }

        // GET: api/Report/dashboard/finance
        [HttpGet("dashboard/finance")]
        public async Task<ActionResult<object>> GetFinanceDashboard()
        {
            var result = await _reportService.GetFinanceDashboardAsync();
            return Ok(result);
        }
    }
}