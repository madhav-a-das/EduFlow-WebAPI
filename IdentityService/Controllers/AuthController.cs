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
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // Reads UserID from the JWT claim
        private int CurrentUserID =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        // =============================================
        //  POST: api/auth/register
        //  Open to all — creates a new user account
        // =============================================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            // ModelState validation → field-level ValidationException
            if (!ModelState.IsValid)
            {
                throw new ValidationException(
                    "VALIDATION_FAILED",
                    "One or more fields are invalid.",
                    ToFieldErrors(ModelState));
            }

            var (success, message, data) = await _authService.RegisterAsync(dto);

            // The service uses string messages to signal which kind of failure;
            // map those to typed exceptions so middleware produces the standard shape.
            if (!success)
            {
                if (message.Contains("already registered", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ConflictException("EMAIL_ALREADY_EXISTS", message);
                }

                throw new ValidationException("REGISTRATION_INVALID", message);
            }

            return Ok(new { message, data });
        }

        // =============================================
        //  POST: api/auth/login
        //  Validates credentials and returns a JWT token
        // =============================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(
                    "VALIDATION_FAILED",
                    "One or more fields are invalid.",
                    ToFieldErrors(ModelState));
            }

            var (success, message, data) = await _authService.LoginAsync(dto);

            if (!success)
            {
                // Account-state failures vs credential failures map to different codes
                if (message.Contains("suspended", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedException("ACCOUNT_SUSPENDED", message);
                }

                if (message.Contains("inactive", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedException("ACCOUNT_INACTIVE", message);
                }

                throw new UnauthorizedException("INVALID_CREDENTIALS", message);
            }

            return Ok(new { message, data });
        }

        // =============================================
        //  POST: api/auth/change-password
        //  Requires login
        // =============================================
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(
                    "VALIDATION_FAILED",
                    "One or more fields are invalid.",
                    ToFieldErrors(ModelState));
            }

            var (success, message) = await _authService.ChangePasswordAsync(CurrentUserID, dto);

            if (!success)
            {
                if (message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotFoundException("USER_NOT_FOUND", "User", CurrentUserID);
                }

                if (message.Contains("incorrect", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedException("CURRENT_PASSWORD_WRONG", message);
                }

                throw new ValidationException("PASSWORD_CHANGE_INVALID", message);
            }

            return Ok(new { message });
        }

        // =============================================
        //  Helper — converts ASP.NET ModelState errors into the
        //  Dictionary<string, string[]> shape ApiErrorResponse.Errors expects.
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
