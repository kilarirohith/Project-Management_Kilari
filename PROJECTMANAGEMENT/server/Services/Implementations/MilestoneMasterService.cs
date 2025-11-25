using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class MilestoneMasterService : IMilestoneMasterService
    {
        private readonly AppDbContext _context;

        public MilestoneMasterService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MilestoneMasterDTO>> GetAllAsync()
        {
            return await _context.MilestoneMasters
                .Select(m => new MilestoneMasterDTO 
                { 
                    Id = m.Id, 
                    Name = m.Name, 
                    Weightage = m.Weightage 
                })
                .OrderByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<MilestoneMasterDTO?> GetByIdAsync(int id)
        {
            var m = await _context.MilestoneMasters.FindAsync(id);
            if (m == null) return null;
            return new MilestoneMasterDTO { Id = m.Id, Name = m.Name, Weightage = m.Weightage };
        }

        public async Task<MilestoneMasterDTO> CreateAsync(MilestoneMasterDTO dto)
        {
            var milestone = new MilestoneMaster
            {
                Name = dto.Name,
                Weightage = dto.Weightage
            };

            _context.MilestoneMasters.Add(milestone);
            await _context.SaveChangesAsync();
            
            dto.Id = milestone.Id;
            return dto;
        }

        public async Task<MilestoneMasterDTO?> UpdateAsync(int id, MilestoneMasterDTO dto)
        {
            var milestone = await _context.MilestoneMasters.FindAsync(id);
            if (milestone == null) return null;

            milestone.Name = dto.Name;
            milestone.Weightage = dto.Weightage;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var milestone = await _context.MilestoneMasters.FindAsync(id);
            if (milestone == null) return false;

            _context.MilestoneMasters.Remove(milestone);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}