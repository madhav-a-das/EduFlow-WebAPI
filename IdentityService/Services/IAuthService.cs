using IdentityService.DTOs;

namespace IdentityService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, TokenResponseDTO? Data)> RegisterAsync(RegisterDTO dto);
        Task<(bool Success, string Message, TokenResponseDTO? Data)> LoginAsync(LoginDTO dto);
        Task<(bool Success, string Message)> ChangePasswordAsync(int userID, ChangePasswordDTO dto);
    }
}