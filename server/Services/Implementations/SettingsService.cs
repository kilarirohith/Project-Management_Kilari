using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class SettingsService : ISettingsService
    {
        private readonly AppDbContext _db;

        public SettingsService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserSettingsDto> GetForUserAsync(int userId)
        {
            var settings = await _db.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);

            if (settings == null)
            {
                settings = new UserSettings { UserId = userId };
                _db.UserSettings.Add(settings);
                await _db.SaveChangesAsync();
                
            }

            return ToDto(settings);
        }

        public async Task<UserSettingsDto> UpsertForUserAsync(int userId, UserSettingsDto dto)
        {
            var settings = await _db.UserSettings
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (settings == null)
            {
                settings = new UserSettings
                {
                    UserId = userId
                };
                _db.UserSettings.Add(settings);
            }

            settings.DarkMode = dto.DarkMode;
            settings.EmailNotifications = dto.EmailNotifications;
            settings.SmsNotifications = dto.SmsNotifications;
            settings.TwoFactorEnabled = dto.TwoFactorEnabled;
            settings.DefaultProjectView = dto.DefaultProjectView ?? "kanban";
            settings.SessionTimeoutMinutes = dto.SessionTimeoutMinutes <= 0 ? 30 : dto.SessionTimeoutMinutes;
            

            await _db.SaveChangesAsync();

            return ToDto(settings);
        }
        private static UserSettingsDto ToDto(UserSettings s) => new()
        {
            DarkMode= s.DarkMode,
            EmailNotifications= s.EmailNotifications,
            SmsNotifications= s.SmsNotifications,
            TwoFactorEnabled = s.TwoFactorEnabled,
            DefaultProjectView = s.DefaultProjectView,
            SessionTimeoutMinutes = s.SessionTimeoutMinutes
        };
    }
}
