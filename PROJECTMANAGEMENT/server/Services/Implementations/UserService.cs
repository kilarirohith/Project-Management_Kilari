using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;
using server.Utils; // Ensure you have AuthHelper here for Password Hashing

namespace server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UserService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // 1. LOGIN
        public async Task<(string? Token, User? User)> LoginUserAsync(LoginDTO dto)
        {
            var user = await _context.Users
                .Include(u => u.Role) // Load Role for permissions
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            // Verify Password using Helper
            if (user == null || !AuthHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return (null, null);

            // Generate JWT Token
            string token = AuthHelper.GenerateJwtToken(user, _config);

            return (token, user);
        }

        // 2. CREATE USER (Used by Admin)
        public async Task<User> RegisterUserAsync(RegisterUserDTO dto)
        {
            // A. Validation: Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email already exists.");

            // B. Validation: Check if Role exists
            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role == null) 
                throw new Exception("Invalid Role ID selected.");

            // C. Create User Object
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Username = dto.Email, // Default username to email
                RoleId = dto.RoleId,
                // SECURITY: Hash the password before saving
                PasswordHash = AuthHelper.HashPassword(dto.Password) 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // 3. GET ALL USERS
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role) // Include Role so we can show "Admin", "User", etc.
                .OrderByDescending(u => u.Id) // Newest first
                .ToListAsync();
        }

        // 4. GET BY ID
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // 5. DELETE
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

    
public async Task<User?> UpdateUserAsync(int id, UpdateUserDTO dto)
{
    var user = await _context.Users.FindAsync(id);
    if (user == null) return null;

    // 1. Update Basic Info
    user.FullName = dto.FullName;
    user.Email = dto.Email;
    user.Username = dto.Email; // syncing username
    user.RoleId = dto.RoleId;

    // 2. Update Password ONLY if provided
    if (!string.IsNullOrEmpty(dto.Password))
    {
        user.PasswordHash = AuthHelper.HashPassword(dto.Password);
    }

    await _context.SaveChangesAsync();
    return user;
}
    }
}