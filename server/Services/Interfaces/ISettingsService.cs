using System.Threading.Tasks;
using server.DTOs;

namespace server.Services.Interfaces
{
    public interface ISettingsService
    {
        Task<UserSettingsDto> GetForUserAsync(int userId);
        Task<UserSettingsDto> UpsertForUserAsync(int userId, UserSettingsDto dto);
    }
}
