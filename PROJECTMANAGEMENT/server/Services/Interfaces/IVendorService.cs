using server.DTOs;

namespace server.Services.Interfaces
{
    public interface IVendorService
    {
        Task<IEnumerable<VendorDTO>> GetAllAsync();
        Task<VendorDTO?> GetByIdAsync(int id);
        Task<VendorDTO> CreateAsync(CreateVendorDTO dto);
        Task<VendorDTO?> UpdateAsync(int id, CreateVendorDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
