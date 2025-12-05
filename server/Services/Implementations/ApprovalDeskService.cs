
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class ApprovalDeskService : IApprovalDeskService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ApprovalDeskService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ---------- helper to delete report file ----------
        private void DeleteReportFileIfExists(VendorWork vw)
        {
            if (string.IsNullOrEmpty(vw.ReportFilePath)) return;

            var root = _env.WebRootPath ?? _env.ContentRootPath;
            var fullPath = Path.Combine(root, vw.ReportFilePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            vw.ReportFilePath = null;
        }

        // ---------- PAGED + FILTERED ----------
        public async Task<(IEnumerable<ApprovalDeskDTO> Items, int TotalCount)> GetPagedAsync(
            string? status,
            string? vendorName,
            string? projectName,
            DateTime? dateFrom,
            DateTime? dateTo,
            int page,
            int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.ApprovalDesks
                .Include(a => a.VendorWork)
                    .ThenInclude(vw => vw.Vendor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(vendorName))
            {
                var vn = vendorName.Trim().ToLower();
                query = query.Where(a => a.VendorWork.Vendor.VendorName.ToLower().Contains(vn));
            }

            if (!string.IsNullOrWhiteSpace(projectName))
            {
                var pn = projectName.Trim().ToLower();
                query = query.Where(a => a.VendorWork.ProjectName.ToLower().Contains(pn));
            }

            if (dateFrom.HasValue)
            {
                var from = dateFrom.Value.Date;
                query = query.Where(a => a.VendorWork.Date >= from);
            }

            if (dateTo.HasValue)
            {
                var to = dateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(a => a.VendorWork.Date <= to);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new ApprovalDeskDTO
                {
                    Id = a.Id,
                    Status = a.Status,
                    Remarks = a.Remarks,
                    VendorWorkId = a.VendorWorkId,
                    Date = a.VendorWork.Date,
                    ProjectName = a.VendorWork.ProjectName,
                    VendorName = a.VendorWork.Vendor.VendorName,
                    Category = a.VendorWork.Category,
                    CoordinatorName = a.VendorWork.CoordinatorName,
                    ReportFilePath = a.VendorWork.ReportFilePath
                })
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<ApprovalDeskDTO>> GetAllAsync()
        {
            var (items, _) = await GetPagedAsync(null, null, null, null, null, 1, int.MaxValue);
            return items;
        }

        public async Task<ApprovalDeskDTO?> GetByIdAsync(int id)
        {
            return await _context.ApprovalDesks
                .Include(a => a.VendorWork)
                    .ThenInclude(vw => vw.Vendor)
                .Where(a => a.Id == id)
                .Select(a => new ApprovalDeskDTO
                {
                    Id = a.Id,
                    Status = a.Status,
                    Remarks = a.Remarks,
                    VendorWorkId = a.VendorWorkId,
                    Date = a.VendorWork.Date,
                    ProjectName = a.VendorWork.ProjectName,
                    VendorName = a.VendorWork.Vendor.VendorName,
                    Category = a.VendorWork.Category,
                    CoordinatorName = a.VendorWork.CoordinatorName,
                    ReportFilePath = a.VendorWork.ReportFilePath
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ApprovalDeskDTO> CreateAsync(CreateApprovalDeskDTO dto)
        {
            var vw = await _context.VendorWorks
                .Include(x => x.Vendor)
                .FirstOrDefaultAsync(x => x.Id == dto.VendorWorkId)
                ?? throw new KeyNotFoundException("Vendor work not found.");

            var approvalDesk = new ApprovalDesk
            {
                VendorWorkId = dto.VendorWorkId,
                Status = dto.Status ?? "Pending",
                Remarks = dto.Remarks ?? string.Empty
            };

            _context.ApprovalDesks.Add(approvalDesk);
            await _context.SaveChangesAsync();

            vw.ApprovalStatus = approvalDesk.Status;

            if (!string.Equals(approvalDesk.Status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                DeleteReportFileIfExists(vw);
            }

            await _context.SaveChangesAsync();

            return new ApprovalDeskDTO
            {
                Id = approvalDesk.Id,
                Status = approvalDesk.Status,
                Remarks = approvalDesk.Remarks,
                VendorWorkId = approvalDesk.VendorWorkId,
                Date = vw.Date,
                ProjectName = vw.ProjectName,
                VendorName = vw.Vendor.VendorName,
                Category = vw.Category,
                CoordinatorName = vw.CoordinatorName,
                ReportFilePath = vw.ReportFilePath
            };
        }

        public async Task<ApprovalDeskDTO?> UpdateAsync(int id, CreateApprovalDeskDTO dto)
        {
            var approvalDesk = await _context.ApprovalDesks
                .Include(a => a.VendorWork)
                    .ThenInclude(vw => vw.Vendor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (approvalDesk == null) return null;

            var vw = approvalDesk.VendorWork;

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                approvalDesk.Status = dto.Status;
            }

            approvalDesk.Remarks = dto.Remarks ?? approvalDesk.Remarks;

            await _context.SaveChangesAsync();

            vw.ApprovalStatus = approvalDesk.Status;

            if (!string.Equals(approvalDesk.Status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                DeleteReportFileIfExists(vw);
            }

            await _context.SaveChangesAsync();

            return new ApprovalDeskDTO
            {
                Id = approvalDesk.Id,
                Status = approvalDesk.Status,
                Remarks = approvalDesk.Remarks,
                VendorWorkId = approvalDesk.VendorWorkId,
                Date = vw.Date,
                ProjectName = vw.ProjectName,
                VendorName = vw.Vendor.VendorName,
                Category = vw.Category,
                CoordinatorName = vw.CoordinatorName,
                ReportFilePath = vw.ReportFilePath
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var approvalDesk = await _context.ApprovalDesks.FindAsync(id);
            if (approvalDesk == null) return false;

            _context.ApprovalDesks.Remove(approvalDesk);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
