using EduFlow.Shared.Errors;
using IdentityService.Data;
using IdentityService.DTOs;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/auditlog")]
    [Authorize]
    public class AuditLogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditLogController(AppDbContext context)
        {
            _context = context;
        }

        // =============================================
        //  GET: api/auditlog
        // =============================================
        [HttpGet]
        [Authorize(Roles = "Administrator,ComplianceOfficer")]
        public async Task<ActionResult<IEnumerable<AuditLogDTO>>> GetAllLogs()
        {
            var logs = await _context.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            var result = logs.Select(a => new AuditLogDTO
            {
                AuditID      = a.AuditID,
                UserID       = a.UserID,
                UserName     = a.User?.Name ?? "Unknown",
                Action       = a.Action,
                ResourceType = a.ResourceType,
                ResourceID   = a.ResourceID,
                Details      = a.Details,
                Timestamp    = a.Timestamp
            });

            return Ok(result);
        }

        // =============================================
        //  GET: api/auditlog/user/5
        // =============================================
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Administrator,ComplianceOfficer")]
        public async Task<ActionResult<IEnumerable<AuditLogDTO>>> GetLogsByUser(int userId)
        {
            var logs = await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.UserID == userId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            var result = logs.Select(a => new AuditLogDTO
            {
                AuditID      = a.AuditID,
                UserID       = a.UserID,
                UserName     = a.User?.Name ?? "Unknown",
                Action       = a.Action,
                ResourceType = a.ResourceType,
                ResourceID   = a.ResourceID,
                Details      = a.Details,
                Timestamp    = a.Timestamp
            });

            return Ok(result);
        }

        // =============================================
        //  GET: api/auditlog/resource/Enrollment
        // =============================================
        [HttpGet("resource/{resourceType}")]
        [Authorize(Roles = "Administrator,ComplianceOfficer")]
        public async Task<ActionResult<IEnumerable<AuditLogDTO>>> GetLogsByResource(string resourceType)
        {
            var logs = await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.ResourceType == resourceType)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            var result = logs.Select(a => new AuditLogDTO
            {
                AuditID      = a.AuditID,
                UserID       = a.UserID,
                UserName     = a.User?.Name ?? "Unknown",
                Action       = a.Action,
                ResourceType = a.ResourceType,
                ResourceID   = a.ResourceID,
                Details      = a.Details,
                Timestamp    = a.Timestamp
            });

            return Ok(result);
        }

        // =============================================
        //  POST: api/auditlog
        //  Other services post here. Audit logs are immutable (no PUT/DELETE).
        // =============================================
        [HttpPost]
        public async Task<ActionResult<AuditLogDTO>> CreateLog(CreateAuditLogDTO dto)
        {
            bool userExists = await _context.Users.AnyAsync(u => u.UserID == dto.UserID);
            if (!userExists)
            {
                throw new NotFoundException("USER_NOT_FOUND", "User", dto.UserID);
            }

            var log = new AuditLog
            {
                UserID       = dto.UserID,
                Action       = dto.Action,
                ResourceType = dto.ResourceType,
                ResourceID   = dto.ResourceID,
                Details      = dto.Details,
                Timestamp    = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(dto.UserID);

            return CreatedAtAction(nameof(GetAllLogs), new AuditLogDTO
            {
                AuditID      = log.AuditID,
                UserID       = log.UserID,
                UserName     = user?.Name ?? "Unknown",
                Action       = log.Action,
                ResourceType = log.ResourceType,
                ResourceID   = log.ResourceID,
                Details      = log.Details,
                Timestamp    = log.Timestamp
            });
        }
    }
}
