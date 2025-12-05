
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

        // ========== PAGED + FILTERED QUERY ==========
        public async Task<PagedResult<VendorWorkDTO>> GetPagedAsync(
            VendorWorkFilterParams filter,
            int? currentVendorId,
            bool isVendor)
        {
            var query = _context.VendorWorks
                .Include(vw => vw.Vendor)
                .AsQueryable();

            if (isVendor && currentVendorId.HasValue)
            {
                query = query.Where(vw => vw.VendorId == currentVendorId.Value);
            }
            else
            {
                if (filter.VendorId.HasValue)
                {
                    query = query.Where(vw => vw.VendorId == filter.VendorId.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.VendorName))
            {
                var vendorName = filter.VendorName.Trim().ToLower();
                query = query.Where(vw =>
                    vw.Vendor.VendorName.ToLower().Contains(vendorName));
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(vw =>
                    vw.ApprovalStatus == filter.Status);
            }

            if (filter.DateFrom.HasValue)
            {
                query = query.Where(vw => vw.Date >= filter.DateFrom.Value);
            }

            if (filter.DateTo.HasValue)
            {
                var end = filter.DateTo.Value.Date.AddDays(1);
                query = query.Where(vw => vw.Date < end);
            }

            var totalCount = await query.CountAsync();

            var page = filter.Page <= 0 ? 1 : filter.Page;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var items = await query
                .OrderByDescending(vw => vw.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(vw => new VendorWorkDTO
                {
                    Id = vw.Id,
                    Date = vw.Date,
                    ProjectName = vw.ProjectName,
                    Category = vw.Category,
                    CoordinatorName = vw.CoordinatorName,
                    Remarks = vw.Remarks,
                    VendorId = vw.VendorId,
                    VendorName = vw.Vendor.VendorName,
                    ApprovalStatus = vw.ApprovalStatus,
                    ReportFilePath = vw.ReportFilePath
                })
                .ToListAsync();

            return new PagedResult<VendorWorkDTO>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<List<VendorWorkDTO>> GetAllAsync()
        {
            return await _context.VendorWorks
                .Include(vw => vw.Vendor)
                .Select(vw => new VendorWorkDTO
                {
                    Id = vw.Id,
                    Date = vw.Date,
                    ProjectName = vw.ProjectName,
                    Category = vw.Category,
                    CoordinatorName = vw.CoordinatorName,
                    Remarks = vw.Remarks,
                    VendorId = vw.VendorId,
                    VendorName = vw.Vendor.VendorName,
                    ApprovalStatus = vw.ApprovalStatus,
                    ReportFilePath = vw.ReportFilePath
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
                Date = vw.Date,
                ProjectName = vw.ProjectName,
                Category = vw.Category,
                CoordinatorName = vw.CoordinatorName,
                Remarks = vw.Remarks,
                VendorId = vw.VendorId,
                VendorName = vw.Vendor.VendorName,
                ApprovalStatus = vw.ApprovalStatus,
                ReportFilePath = vw.ReportFilePath
            };
        }

        public async Task<VendorWorkDTO> CreateAsync(CreateVendorWorkDTO dto)
        {
            var vendor = await _context.Vendors.FindAsync(dto.VendorId)
                ?? throw new KeyNotFoundException("Vendor not found.");

            var entity = new VendorWork
            {
                Date = dto.Date,
                ProjectName = dto.ProjectName,
                Category = dto.Category,
                CoordinatorName = dto.CoordinatorName,
                Remarks = dto.Remarks,
                VendorId = dto.VendorId,
                ApprovalStatus = "Pending",
                ReportFilePath = null
            };

            _context.VendorWorks.Add(entity);
            await _context.SaveChangesAsync();

            
            var approval = new ApprovalDesk
            {
                VendorWorkId = entity.Id,
                Status = "Pending",
                Remarks = string.Empty
            };

            _context.ApprovalDesks.Add(approval);
            await _context.SaveChangesAsync();

            return new VendorWorkDTO
            {
                Id = entity.Id,
                Date = entity.Date,
                ProjectName = entity.ProjectName,
                Category = entity.Category,
                CoordinatorName = entity.CoordinatorName,
                Remarks = entity.Remarks,
                VendorId = entity.VendorId,
                VendorName = vendor.VendorName,
                ApprovalStatus = entity.ApprovalStatus,
                ReportFilePath = entity.ReportFilePath
            };
        }

        public async Task<VendorWorkDTO?> UpdateAsync(int id, CreateVendorWorkDTO dto)
        {
            var entity = await _context.VendorWorks.FindAsync(id);
            if (entity == null) return null;

            var vendor = await _context.Vendors.FindAsync(dto.VendorId)
                ?? throw new KeyNotFoundException("Vendor not found.");

            entity.Date = dto.Date;
            entity.ProjectName = dto.ProjectName;
            entity.Category = dto.Category;
            entity.CoordinatorName = dto.CoordinatorName;
            entity.Remarks = dto.Remarks;
            entity.VendorId = dto.VendorId;

            await _context.SaveChangesAsync();

            return new VendorWorkDTO
            {
                Id = entity.Id,
                Date = entity.Date,
                ProjectName = entity.ProjectName,
                Category = entity.Category,
                CoordinatorName = entity.CoordinatorName,
                Remarks = entity.Remarks,
                VendorId = entity.VendorId,
                VendorName = vendor.VendorName,
                ApprovalStatus = entity.ApprovalStatus,
                ReportFilePath = entity.ReportFilePath
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.VendorWorks.FindAsync(id);
            if (entity == null) return false;

            _context.VendorWorks.Remove(entity);

            var approvals = _context.ApprovalDesks.Where(a => a.VendorWorkId == id);
            _context.ApprovalDesks.RemoveRange(approvals);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
