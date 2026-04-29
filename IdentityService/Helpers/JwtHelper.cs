using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityService.Models;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        // Generates a signed JWT token for the given User.
        // Embeds UserID, Email, Name, and Role as claims.
        // The Role claim is what makes [Authorize(Roles="Admin")] work on controllers.
        public string GenerateToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var issuer = jwtSettings["Issuer"]!;
            var audience = jwtSettings["Audience"]!;
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                // NameIdentifier = UserID (int) — read via User.FindFirstValue(ClaimTypes.NameIdentifier)
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email,          user.Email),
                new Claim(ClaimTypes.Name,           user.Name),
                // Role claim — enables [Authorize(Roles="Administrator")] etc.
                new Claim(ClaimTypes.Role,           user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetExpiry()
        {
            var minutes = int.Parse(_config["JwtSettings:ExpiryMinutes"]!);
            return DateTime.UtcNow.AddMinutes(minutes);
        }
    }
}