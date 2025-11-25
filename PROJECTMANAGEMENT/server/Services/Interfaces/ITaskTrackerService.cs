using server.DTOs;
using server.Models;

namespace server.Services.Interfaces
{
    public interface ITaskTrackerService
    {
        Task<IEnumerable<TaskTracker>> GetAllAsync();
        Task<TaskTracker> CreateOrUpdateAsync(TaskTrackerDTO dto);
    }
}
