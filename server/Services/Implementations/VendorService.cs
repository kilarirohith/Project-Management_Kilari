using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class VendorService : IVendorService
    {
        private readonly AppDbContext _context;

        public VendorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VendorDTO>> GetAllAsync()
        {
            return await _context.Vendors
                .Select(v => new VendorDTO
                {
                    Id = v.Id,
                    VendorName = v.VendorName,
                    VendorLocation = v.VendorLocation,
                    VendorGst = v.VendorGst,
                    Email = v.Email,
                    Spoc = v.Spoc,
                    UserId = v.UserId
                })
                .ToListAsync();
        }

        public async Task<VendorDTO?> GetByIdAsync(int id)
        {
            var v = await _context.Vendors.FindAsync(id);
            if (v == null) return null;

            return new VendorDTO
            {
                Id = v.Id,
                VendorName = v.VendorName,
                VendorLocation = v.VendorLocation,
                VendorGst = v.VendorGst,
                Email = v.Email,
                Spoc = v.Spoc,
                UserId = v.UserId
            };
        }

        public async Task<VendorDTO> CreateAsync(CreateVendorDTO dto)
        {
            var entity = new Vendor
            {
                VendorName = dto.VendorName,
                VendorLocation = dto.VendorLocation,
                VendorGst = dto.VendorGst,
                Email = dto.Email,
                Spoc = dto.Spoc,
                UserId = dto.UserId
            };

            _context.Vendors.Add(entity);
            await _context.SaveChangesAsync();

            return new VendorDTO
            {
                Id = entity.Id,
                VendorName = entity.VendorName,
                VendorLocation = entity.VendorLocation,
                VendorGst = entity.VendorGst,
                Email = entity.Email,
                Spoc = entity.Spoc,
                UserId = entity.UserId
            };
        }

        public async Task<VendorDTO?> UpdateAsync(int id, CreateVendorDTO dto)
        {
            var entity = await _context.Vendors.FindAsync(id);
            if (entity == null) return null;

            entity.VendorName = dto.VendorName;
            entity.VendorLocation = dto.VendorLocation;
            entity.VendorGst = dto.VendorGst;
            entity.Email = dto.Email;
            entity.Spoc = dto.Spoc;
            entity.UserId = dto.UserId;

            await _context.SaveChangesAsync();

            return new VendorDTO
            {
                Id = entity.Id,
                VendorName = entity.VendorName,
                VendorLocation = entity.VendorLocation,
                VendorGst = entity.VendorGst,
                Email = entity.Email,
                Spoc = entity.Spoc,
                UserId = entity.UserId
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Vendors.FindAsync(id);
            if (entity == null) return false;

            _context.Vendors.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
