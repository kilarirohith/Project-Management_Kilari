using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class VendorWorkService : IVendorWorkService
    {
        private readonly AppDbContext _context;

        public VendorWorkService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VendorWorkDTO>> GetAllAsync()
        {
            return await _context.VendorWorks
                .Include(vw => vw.Vendor)
                .Select(vw => new VendorWorkDTO
                {
                    Id = vw.Id,
                    ProjectName = vw.ProjectName,
                    WorkDescription = vw.WorkDescription,
                    StartDate = vw.StartDate,
                    EndDate = vw.EndDate,
                    Status = vw.Status,
                    VendorId = vw.VendorId,
                    VendorName = vw.Vendor.VendorName
                })
                .ToListAsync();
        }

        public async Task<VendorWorkDTO?> GetByIdAsync(int id)
        {
            var vw = await _context.VendorWorks
                .Include(v => v.Vendor)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vw == null) return null;

            return new VendorWorkDTO
            {
                Id = vw.Id,
                ProjectName = vw.ProjectName,
                WorkDescription = vw.WorkDescription,
                StartDate = vw.StartDate,
                EndDate = vw.EndDate,
                Status = vw.Status,
                VendorId = vw.VendorId,
                VendorName = vw.Vendor.VendorName
            };
        }

        public async Task<VendorWorkDTO> CreateAsync(CreateVendorWorkDTO dto)
        {
            var vendor = await _context.Vendors.FindAsync(dto.VendorId);
            if (vendor == null)
                throw new KeyNotFoundException("Vendor not found.");

            var entity = new VendorWork
            {
                VendorId = dto.VendorId,
                ProjectName = dto.ProjectName,
                WorkDescription = dto.WorkDescription,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status
            };

            _context.VendorWorks.Add(entity);
            await _context.SaveChangesAsync();

            return new VendorWorkDTO
            {
                Id = entity.Id,
                ProjectName = entity.ProjectName,
                WorkDescription = entity.WorkDescription,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = entity.Status,
                VendorId = entity.VendorId,
                VendorName = vendor.VendorName
            };
        }

        public async Task<VendorWorkDTO?> UpdateAsync(int id, CreateVendorWorkDTO dto)
        {
            var entity = await _context.VendorWorks.FindAsync(id);
            if (entity == null) return null;

            var vendor = await _context.Vendors.FindAsync(dto.VendorId);
            if (vendor == null)
                throw new KeyNotFoundException("Vendor not found.");

            entity.VendorId = dto.VendorId;
            entity.ProjectName = dto.ProjectName;
            entity.WorkDescription = dto.WorkDescription;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.Status = dto.Status;

            await _context.SaveChangesAsync();

            return new VendorWorkDTO
            {
                Id = entity.Id,
                ProjectName = entity.ProjectName,
                WorkDescription = entity.WorkDescription,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = entity.Status,
                VendorId = entity.VendorId,
                VendorName = vendor.VendorName
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.VendorWorks.FindAsync(id);
            if (entity == null) return false;

            _context.VendorWorks.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
