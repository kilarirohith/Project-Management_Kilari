using server.DTOs;

namespace server.Services.Interfaces
{
    public interface IVendorWorkService
    {
        
        Task<List<VendorWorkDTO>> GetAllAsync();
        Task<VendorWorkDTO?> GetByIdAsync(int id);
        Task<VendorWorkDTO> CreateAsync(CreateVendorWorkDTO dto);
        Task<VendorWorkDTO?> UpdateAsync(int id, CreateVendorWorkDTO dto);
        Task<bool> DeleteAsync(int id);

        
        Task<PagedResult<VendorWorkDTO>> GetPagedAsync(
            VendorWorkFilterParams filter,
            int? currentVendorId,
            bool isVendor);
    }
}
