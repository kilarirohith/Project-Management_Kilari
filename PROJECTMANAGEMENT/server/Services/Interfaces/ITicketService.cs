using server.DTOs;

namespace server.Services.Interfaces
{
    public interface ITicketService
    {
        Task<List<TicketDTO>> GetAllAsync();
        Task<TicketDTO?> GetByIdAsync(int id);
        Task<TicketDTO> CreateAsync(CreateTicketDTO dto);
        Task<TicketDTO?> UpdateAsync(int id, CreateTicketDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
