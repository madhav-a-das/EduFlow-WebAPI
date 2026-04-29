using IdentityService.DTOs;
using IdentityService.Helpers;
using IdentityService.Models;
using IdentityService.Repositories.Interfaces;
using IdentityService.Services.Interfaces;

namespace IdentityService.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IAuditLogRepository _auditRepo;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<AuthService> _logger;

        private static readonly string[] AllowedRoles =
        {
            "Student", "Faculty", "Registrar",
            "DepartmentHead", "FinanceOfficer",
            "ComplianceOfficer", "Administrator"
        };

        public AuthService(
            IUserRepository userRepo,
            IAuditLogRepository auditRepo,
            JwtHelper jwtHelper,
            ILogger<AuthService> logger)
        {
            _userRepo = userRepo;
            _auditRepo = auditRepo;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, TokenResponseDTO? Data)> RegisterAsync(RegisterDTO dto)
        {
            // Validate role against the EduFlow role list from the requirement
            if (!AllowedRoles.Contains(dto.Role))
                return (false, $"Role must be one of: {string.Join(", ", AllowedRoles)}", null);

            // Check email uniqueness
            if (await _userRepo.EmailExistsAsync(dto.Email))
            {
                _logger.LogWarning("Register attempt with existing email: {Email}", dto.Email);
                return (false, "Email is already registered.", null);
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = dto.Role,
                Status = "Active",
                // BCrypt.HashPassword handles salting automatically
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepo.CreateAsync(user);

            _logger.LogInformation("User registered: {Email} [{Role}]", dto.Email, dto.Role);

            // Write audit log for this registration
            await _auditRepo.AddAsync(new AuditLog
            {
                UserID = user.UserID,
                Action = "REGISTER",
                ResourceType = "User",
                ResourceID = user.UserID,
                Details = $"New user registered: {user.Name} with role {user.Role}",
                Timestamp = DateTime.UtcNow
            });

            var token = _jwtHelper.GenerateToken(user);

            return (true, "Registration successful.", new TokenResponseDTO
            {
                //Token = token,
                UserID = user.UserID,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = _jwtHelper.GetExpiry()
            });
        }

        public async Task<(bool Success, string Message, TokenResponseDTO? Data)> LoginAsync(LoginDTO dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);

            if (user == null)
            {
                _logger.LogWarning("Login failed — unknown email: {Email}", dto.Email);
                // Return same message for both "not found" and "wrong password"
                // to avoid leaking which emails are registered
                return (false, "Invalid email or password.", null);
            }

            if (user.Status == "Suspended")
            {
                _logger.LogWarning("Suspended user login attempt: {Email}", dto.Email);
                return (false, "Your account has been suspended. Contact the administrator.", null);
            }

            if (user.Status == "Inactive")
                return (false, "Your account is inactive. Contact the administrator.", null);

            // BCrypt.Verify compares the plain password against the stored hash
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for: {Email}", dto.Email);
                return (false, "Invalid email or password.", null);
            }

            _logger.LogInformation("User logged in: {Email} [{Role}]", dto.Email, user.Role);

            // Log the login action in the audit trail
            await _auditRepo.AddAsync(new AuditLog
            {
                UserID = user.UserID,
                Action = "LOGIN",
                ResourceType = "User",
                ResourceID = user.UserID,
                Details = $"User {user.Name} logged in successfully",
                Timestamp = DateTime.UtcNow
            });

            var token = _jwtHelper.GenerateToken(user);

            return (true, "Login successful.", new TokenResponseDTO
            {
                Token = token,
                UserID = user.UserID,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = _jwtHelper.GetExpiry()
            });
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userID, ChangePasswordDTO dto)
        {
            var user = await _userRepo.GetByIdAsync(userID);
            if (user == null) return (false, "User not found.");

            // Verify current password before allowing change
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return (false, "Current password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateAsync(user);

            _logger.LogInformation("Password changed for UserID: {UserID}", userID);

            await _auditRepo.AddAsync(new AuditLog
            {
                UserID = userID,
                Action = "PASSWORD_CHANGE",
                ResourceType = "User",
                ResourceID = userID,
                Details = "User changed their password",
                Timestamp = DateTime.UtcNow
            });

            return (true, "Password changed successfully.");
        }
    }
}