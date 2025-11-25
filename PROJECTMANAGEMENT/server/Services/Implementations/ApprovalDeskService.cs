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

        public ApprovalDeskService(AppDbContext context)
        {
            _context = context;
        }

        // ------------------------------
        // GET ALL APPROVAL DESKS
        // ------------------------------
        public async Task<IEnumerable<ApprovalDesk>> GetAllAsync()
        {
            return await _context.ApprovalDesks
                .Include(a => a.Project)
                .Include(a => a.VendorWork)
                .ThenInclude(vw => vw.Vendor)
                .ToListAsync();
        }

        // ------------------------------
        // GET APPROVAL DESK BY ID
        // ------------------------------
        public async Task<ApprovalDesk?> GetByIdAsync(int id)
        {
            return await _context.ApprovalDesks
                .Include(a => a.Project)
                .Include(a => a.VendorWork)
                .ThenInclude(vw => vw.Vendor)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // ------------------------------
        // CREATE NEW APPROVAL DESK
        // ------------------------------
        public async Task<ApprovalDesk> CreateAsync(CreateApprovalDeskDTO dto)
        {
            var approvalDesk = new ApprovalDesk
            {
                Status = dto.Status ?? "Pending",
                Remarks = dto.Remarks ?? string.Empty,
                ProjectId = dto.ProjectId,
                VendorWorkId = dto.VendorWorkId
            };

            _context.ApprovalDesks.Add(approvalDesk);
            await _context.SaveChangesAsync();
            return approvalDesk;
        }

        // ------------------------------
        // UPDATE EXISTING APPROVAL DESK
        // ------------------------------
        public async Task<ApprovalDesk?> UpdateAsync(int id, CreateApprovalDeskDTO dto)
        {
            var approvalDesk = await _context.ApprovalDesks.FindAsync(id);
            if (approvalDesk == null) return null;

            approvalDesk.Status = dto.Status ?? approvalDesk.Status;
            approvalDesk.Remarks = dto.Remarks ?? approvalDesk.Remarks;
            approvalDesk.ProjectId = dto.ProjectId;
            approvalDesk.VendorWorkId = dto.VendorWorkId;

            _context.ApprovalDesks.Update(approvalDesk);
            await _context.SaveChangesAsync();
            return approvalDesk;
        }

        // ------------------------------
        // DELETE APPROVAL DESK
        // ------------------------------
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
