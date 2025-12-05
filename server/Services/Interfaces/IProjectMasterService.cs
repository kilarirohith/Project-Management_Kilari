using server.DTOs;

namespace server.Services.Interfaces
{
    public interface IProjectMasterService
    {
        Task<List<ProjectMasterDTO>> GetAllAsync();
        Task<ProjectMasterDTO?> GetByIdAsync(int id);
        Task<ProjectMasterDTO> CreateAsync(CreateProjectMasterDTO dto);
        Task<ProjectMasterDTO?> UpdateAsync(int id, CreateProjectMasterDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}