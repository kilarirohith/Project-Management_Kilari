using server.DTOs;

namespace server.Services.Interfaces
{
    public interface IClientService
    {
        Task<List<ClientDTO>> GetAllAsync();
        Task<ClientDTO?> GetByIdAsync(int id);
        Task<ClientDTO> CreateAsync(ClientDTO dto);
        Task<ClientDTO?> UpdateAsync(int id, ClientDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}