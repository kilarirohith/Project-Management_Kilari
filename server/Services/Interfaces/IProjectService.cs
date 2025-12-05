
using System.Collections.Generic;
using System.Threading.Tasks;
using server.DTOs;

namespace server.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<ProjectDTO>> GetAllAsync();
        Task<ProjectDTO?> GetByIdAsync(int id);
        Task<ProjectDTO> CreateAsync(CreateProjectDTO dto);
        Task<ProjectDTO?> UpdateAsync(int id, UpdateProjectDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
