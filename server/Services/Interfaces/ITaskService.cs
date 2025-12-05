
using server.DTOs;
using server.Models;

namespace server.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<ProjectTask>> GetAllAsync();
        Task<ProjectTask?> GetByIdAsync(int id);
        Task<ProjectTask> CreateAsync(CreateTaskDTO dto);
        Task<ProjectTask?> UpdateAsync(int id, CreateTaskDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
