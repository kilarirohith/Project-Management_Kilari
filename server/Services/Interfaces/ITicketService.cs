using System.Collections.Generic;
using System.Threading.Tasks;
using server.DTOs;

namespace server.Services.Interfaces
{
    public interface ITicketService
    {
            Task<IEnumerable<TicketDTO>> GetAllAsync();
            Task<TicketDTO?> GetByIdAsync(int id);
            Task<TicketDTO> CreateAsync(CreateTicketDTO dto, int currentUserId);
            Task<TicketDTO?> UpdateAsync(int id, CreateTicketDTO dto, int currentUserId);
            Task<bool> DeleteAsync(int id);
    }
}
