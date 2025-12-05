
using server.DTOs;

namespace server.Services.Interfaces
{
    public interface IProfileService
    {
        Task<UserProfileDto?> GetProfileAsync(int userId);

        Task<UserProfileDto> UpdateProfileAsync(
            int userId,
            string fullName,
            string email,
            string? avatarUrl,
            bool removeAvatar);
    }
}
