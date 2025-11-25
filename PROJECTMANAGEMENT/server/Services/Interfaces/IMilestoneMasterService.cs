using server.DTOs;

namespace server.Services.Interfaces
{
   public interface IMilestoneMasterService
{
    Task<List<MilestoneMasterDTO>> GetAllAsync();
    Task<MilestoneMasterDTO?> GetByIdAsync(int id);
    Task<MilestoneMasterDTO> CreateAsync(MilestoneMasterDTO dto);
    Task<MilestoneMasterDTO?> UpdateAsync(int id, MilestoneMasterDTO dto);
    Task<bool> DeleteAsync(int id);
}

}