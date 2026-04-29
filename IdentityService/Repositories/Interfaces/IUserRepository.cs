using IdentityService.Models;

namespace IdentityService.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int userID);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByRoleAsync(string role);
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> EmailExistsAsync(string email);
    }
}