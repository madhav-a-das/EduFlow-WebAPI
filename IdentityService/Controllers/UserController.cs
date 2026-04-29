using System.Security.Claims;
using EduFlow.Shared.Errors;
using IdentityService.DTOs;
using IdentityService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]   // all endpoints require a valid JWT
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        private int CurrentUserID =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        // =============================================
        //  GET: api/user
        //  Administrator only
        // =============================================
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // =============================================
        //  GET: api/user/{id}
        //  Administrator can view anyone; others can view only themselves
        // =============================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var isAdmin = User.IsInRole("Administrator");
            if (!isAdmin && CurrentUserID != id)
            {
                throw new ForbiddenException(
                    "CROSS_USER_ACCESS",
                    "You may only view your own profile.");
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("USER_NOT_FOUND", "User", id);
            }

            return Ok(user);
        }

        // =============================================
        //  GET: api/user/role/{role}
        //  Administrator and Registrar can list users by role
        // =============================================
        [HttpGet("role/{role}")]
        [Authorize(Roles = "Administrator,Registrar")]
        public async Task<IActionResult> GetByRole(string role)
        {
            var users = await _userService.GetUsersByRoleAsync(role);
            return Ok(users);
        }

        // =============================================
        //  PUT: api/user/{id}
        //  Administrator can update anyone; others can update only themselves
        // =============================================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO dto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(
                    "VALIDATION_FAILED",
                    "One or more fields are invalid.",
                    ToFieldErrors(ModelState));
            }

            var isAdmin = User.IsInRole("Administrator");
            if (!isAdmin && CurrentUserID != id)
            {
                throw new ForbiddenException(
                    "CROSS_USER_ACCESS",
                    "You may only update your own profile.");
            }

            var (success, message) = await _userService.UpdateUserAsync(id, dto);
            if (!success)
            {
                throw new NotFoundException("USER_NOT_FOUND", "User", id);
            }

            await _userService.LogActionAsync(CurrentUserID, "UPDATE", "User", id,
                $"Updated profile of UserID {id}");

            return Ok(new { message });
        }

        // =============================================
        //  DELETE: api/user/{id}
        //  Administrator only — soft delete
        // =============================================
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var (success, message) = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                throw new NotFoundException("USER_NOT_FOUND", "User", id);
            }

            await _userService.LogActionAsync(CurrentUserID, "DELETE", "User", id,
                $"Soft-deleted UserID {id}");

            return Ok(new { message });
        }

        // =============================================
        //  POST: api/user/assign-role
        //  Administrator only
        // =============================================
        [HttpPost("assign-role")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(
                    "VALIDATION_FAILED",
                    "One or more fields are invalid.",
                    ToFieldErrors(ModelState));
            }

            var (success, message) = await _userService.AssignRoleAsync(dto);
            if (!success)
            {
                if (message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotFoundException("USER_NOT_FOUND", "User", dto.UserID);
                }
                throw new ValidationException("ROLE_ASSIGNMENT_INVALID", message);
            }

            await _userService.LogActionAsync(CurrentUserID, "ASSIGN_ROLE", "User", dto.UserID,
                $"Assigned role '{dto.Role}' to UserID {dto.UserID}");

            return Ok(new { message });
        }

        // =============================================
        //  GET: api/user/auditlogs
        //  Administrator and ComplianceOfficer
        // =============================================
        [HttpGet("auditlogs")]
        [Authorize(Roles = "Administrator,ComplianceOfficer")]
        public async Task<IActionResult> GetAuditLogs()
        {
            var logs = await _userService.GetAllLogsAsync();
            return Ok(logs);
        }

        // =============================================
        //  GET: api/user/auditlogs/user/{userID}
        // =============================================
        [HttpGet("auditlogs/user/{userID:int}")]
        [Authorize(Roles = "Administrator,ComplianceOfficer")]
        public async Task<IActionResult> GetLogsByUser(int userID)
        {
            var logs = await _userService.GetLogsByUserAsync(userID);
            return Ok(logs);
        }

        // =============================================
        //  GET: api/user/auditlogs/resource/{resourceType}
        // =============================================
        [HttpGet("auditlogs/resource/{resourceType}")]
        [Authorize(Roles = "Administrator,ComplianceOfficer")]
        public async Task<IActionResult> GetLogsByResource(string resourceType)
        {
            var logs = await _userService.GetLogsByResourceAsync(resourceType);
            return Ok(logs);
        }

        // =============================================
        //  Helper — converts ModelState errors into ApiErrorResponse.Errors shape
        // =============================================
        private static Dictionary<string, string[]> ToFieldErrors(ModelStateDictionary modelState)
        {
            return modelState
                .Where(kvp => kvp.Value!.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());
        }
    }
}
