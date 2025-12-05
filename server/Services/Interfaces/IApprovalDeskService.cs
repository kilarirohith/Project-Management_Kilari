
using server.DTOs;

public interface IApprovalDeskService
{
    Task<(IEnumerable<ApprovalDeskDTO> Items, int TotalCount)> GetPagedAsync(
        string? status,
        string? vendorName,
        string? projectName,
        DateTime? dateFrom,
        DateTime? dateTo,
        int page,
        int pageSize);

    Task<IEnumerable<ApprovalDeskDTO>> GetAllAsync();   
    Task<ApprovalDeskDTO?> GetByIdAsync(int id);
    Task<ApprovalDeskDTO> CreateAsync(CreateApprovalDeskDTO dto);
    Task<ApprovalDeskDTO?> UpdateAsync(int id, CreateApprovalDeskDTO dto);
    Task<bool> DeleteAsync(int id);
}
