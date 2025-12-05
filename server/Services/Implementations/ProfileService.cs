// server/Services/Implementations/ProfileService.cs
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _db;

        public ProfileService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserProfileDto?> GetProfileAsync(int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                Id        = user.Id,
                FullName  = user.FullName,
                Email     = user.Email,
                AvatarUrl = user.AvatarUrl
            };
        }

        public async Task<UserProfileDto> UpdateProfileAsync(
            int userId, string fullName, string email, string? avatarUrl, bool removeAvatar)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId)
                       ?? throw new KeyNotFoundException("User not found");

            user.FullName = fullName;
            user.Email    = email;

            // delete / clear avatar
            if (removeAvatar)
            {
                user.AvatarUrl = null;
            }

            // set new avatar if uploaded
            if (avatarUrl != null)
            {
                user.AvatarUrl = avatarUrl;
            }

            await _db.SaveChangesAsync();

            return new UserProfileDto
            {
                Id        = user.Id,
                FullName  = user.FullName,
                Email     = user.Email,
                AvatarUrl = user.AvatarUrl
            };
        }
    }
}
