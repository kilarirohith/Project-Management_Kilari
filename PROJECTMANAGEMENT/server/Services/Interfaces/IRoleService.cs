using server.DTOs;

namespace server.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllAsync();
        Task<RoleDTO?> GetByIdAsync(int id);
        Task<RoleDTO> CreateAsync(RoleDTO roleDTO);
        Task<RoleDTO?> UpdateAsync(int id, RoleDTO roleDTO);
        Task<bool> DeleteAsync(int id);
    }
}
