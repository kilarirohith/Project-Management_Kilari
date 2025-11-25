using server.DTOs;
using server.Models;

namespace server.Services.Interfaces
{
    public interface IUserService
    {
        Task<(string? Token, User? User)> LoginUserAsync(LoginDTO dto);
        Task<User> RegisterUserAsync(RegisterUserDTO dto);
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> UpdateUserAsync(int id, UpdateUserDTO dto); // ✅ Added Update
        Task<bool> DeleteUserAsync(int id);
    }
}