using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using System.Security.Cryptography;
using server.Models;
using server.Services.Interfaces;
using server.Utils;

namespace server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        private readonly IEmailService _emailService;
public UserService(AppDbContext context, IConfiguration config, IEmailService emailService)
{
    _context = context;
    _config = config;
    _emailService = emailService;
}


       public async Task<(string? Token, User? User)> LoginUserAsync(LoginDTO dto)
{
    var user = await _context.Users
        .Include(u => u.Role)
            .ThenInclude(r => r.RolePermissions)
        .Include(u => u.Settings)
        .FirstOrDefaultAsync(u => u.Email == dto.Email);

    if (user == null || !AuthHelper.VerifyPassword(dto.Password, user.PasswordHash))
        return (null, null);

  
    if (user.Settings == null)
    {
        user.Settings = new UserSettings
        {
            UserId = user.Id,
            SessionTimeoutMinutes = 30 
        };
        _context.UserSettings.Add(user.Settings);
        await _context.SaveChangesAsync();
    }

    
    int sessionTimeout = user.Settings.SessionTimeoutMinutes > 0 
        ? user.Settings.SessionTimeoutMinutes 
        : 30;

    
    var token = AuthHelper.GenerateJwtToken(user, sessionTimeout, _config);

    return (token, user);
}


        public async Task<User> RegisterUserAsync(RegisterUserDTO dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email already exists.");

            var role = await _context.Roles.FindAsync(dto.RoleId)
                       ?? throw new Exception("Invalid Role ID");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Username = dto.Email,
                RoleId = dto.RoleId,
                PasswordHash = AuthHelper.HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .OrderByDescending(u => u.Id)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

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

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Username = dto.Email;
            user.RoleId = dto.RoleId;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash = AuthHelper.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task RequestPasswordResetAsync(string email)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null) return;

    var tokenBytes = new byte[32];
    RandomNumberGenerator.Fill(tokenBytes);
    var token = Convert.ToBase64String(tokenBytes)
        .Replace("+", "-").Replace("/", "_").TrimEnd('=');

    var reset = new PasswordResetToken
    {
        UserId = user.Id,
        Token = token,
        ExpiresAt = DateTime.UtcNow.AddHours(1)
    };

    _context.PasswordResetTokens.Add(reset);
    await _context.SaveChangesAsync();

    var link = $"{_config["App:FrontendBaseUrl"]}/reset-password?token={token}";

    await _emailService.SendEmailAsync(
        user.Email,
        "PMS Password Reset",
        $"<p>Hello {user.FullName},</p><p>Click to reset: <a href='{link}'>Reset Password</a></p>"
    );
}

public async Task<bool> ResetPasswordAsync(string token, string newPassword)
{
    var reset = await _context.PasswordResetTokens
        .Include(x => x.User)
        .FirstOrDefaultAsync(x => x.Token == token);

    if (reset == null || reset.Used || reset.ExpiresAt < DateTime.UtcNow)
        return false;

    reset.User.PasswordHash = AuthHelper.HashPassword(newPassword);
    reset.Used = true;

    await _context.SaveChangesAsync();
    return true;
}

    }
}
