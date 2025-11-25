using server.DTOs;
using server.Models;

namespace server.Services.Interfaces
{
    public interface IApprovalDeskService
    {
        Task<IEnumerable<ApprovalDesk>> GetAllAsync();
        Task<ApprovalDesk?> GetByIdAsync(int id);
        Task<ApprovalDesk> CreateAsync(CreateApprovalDeskDTO dto);
        Task<ApprovalDesk?> UpdateAsync(int id, CreateApprovalDeskDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
