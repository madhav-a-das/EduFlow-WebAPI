using Microsoft.AspNetCore.Mvc;
using ReportingService.Models;
using ReportingService.Services.Interfaces;

namespace ReportingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditPackageController : ControllerBase
    {
        private readonly IAuditPackageService _auditPackageService;

        public AuditPackageController(IAuditPackageService auditPackageService)
        {
            _auditPackageService = auditPackageService;
        }

        // GET: api/AuditPackage
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditPackage>>> GetAllAuditPackages()
        {
            var packages = await _auditPackageService.GetAllAsync();
            return Ok(packages);
        }

        // GET: api/AuditPackage/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuditPackage>> GetAuditPackage(int id)
        {
            var package = await _auditPackageService.GetByIdAsync(id);
            if (package == null) return NotFound();
            return Ok(package);
        }

        // GET: api/AuditPackage/range?start=2025-01-01&end=2025-06-30
        [HttpGet("range")]
        public async Task<ActionResult<IEnumerable<AuditPackage>>> GetByDateRange(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var packages = await _auditPackageService.GetByDateRangeAsync(start, end);
            return Ok(packages);
        }

        // POST: api/AuditPackage
        [HttpPost]
        public async Task<ActionResult<AuditPackage>> CreateAuditPackage(AuditPackage package)
        {
            var created = await _auditPackageService.CreateAsync(package);
            return CreatedAtAction(nameof(GetAuditPackage), new { id = created.PackageID }, created);
        }

        // PUT: api/AuditPackage/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuditPackage(int id, AuditPackage package)
        {
            if (id != package.PackageID) return BadRequest();
            if (!await _auditPackageService.ExistsAsync(id)) return NotFound();

            await _auditPackageService.UpdateAsync(package);
            return NoContent();
        }

        // DELETE: api/AuditPackage/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuditPackage(int id)
        {
            if (!await _auditPackageService.ExistsAsync(id)) return NotFound();

            await _auditPackageService.DeleteAsync(id);
            return NoContent();
        }
    }
}