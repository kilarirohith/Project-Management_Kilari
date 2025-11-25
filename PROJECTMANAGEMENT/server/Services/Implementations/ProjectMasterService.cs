using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class ProjectMasterService : IProjectMasterService
    {
        private readonly AppDbContext _context;

        // ✅ FIXED: Constructor name matches Class name
        public ProjectMasterService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectMasterDTO>> GetAllAsync()
        {
            // ✅ CHANGED: _context.Projects -> _context.ProjectMasters
            return await _context.ProjectMasters
                .Include(p => p.Client)
                .Select(p => new ProjectMasterDTO
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    ClientId = p.ClientId,
                    ClientName = p.Client.ClientName
                })
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<ProjectMasterDTO?> GetByIdAsync(int id)
        {
            var project = await _context.ProjectMasters
                .Include(p => p.Client)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return null;

            return new ProjectMasterDTO
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                Description = project.Description,
                ClientId = project.ClientId,
                ClientName = project.Client.ClientName
            };
        }

        public async Task<ProjectMasterDTO> CreateAsync(CreateProjectMasterDTO dto)
        {
            var client = await _context.Clients.FindAsync(dto.ClientId);
            if (client == null) throw new Exception("Invalid Client ID");

            var projectMaster = new ProjectMaster
            {
                ProjectName = dto.ProjectName,
                Description = dto.Description,
                ClientId = dto.ClientId
            };

            _context.ProjectMasters.Add(projectMaster);
            await _context.SaveChangesAsync();

            // ✅ FIXED: Variable mismatch (project.Id -> projectMaster.Id)
            return new ProjectMasterDTO 
            {
                Id = projectMaster.Id,
                ProjectName = projectMaster.ProjectName,
                Description = projectMaster.Description,
                ClientId = projectMaster.ClientId,
                ClientName = client.ClientName
            };
        }

        public async Task<ProjectMasterDTO?> UpdateAsync(int id, CreateProjectMasterDTO dto)
        {
            var project = await _context.ProjectMasters.FindAsync(id);
            if (project == null) return null;

            var client = await _context.Clients.FindAsync(dto.ClientId);
            if (client == null) throw new Exception("Invalid Client ID");

            project.ProjectName = dto.ProjectName;
            project.Description = dto.Description;
            project.ClientId = dto.ClientId;

            await _context.SaveChangesAsync();

            return new ProjectMasterDTO
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                Description = project.Description,
                ClientId = project.ClientId,
                ClientName = client.ClientName
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _context.ProjectMasters.FindAsync(id);
            if (project == null) return false;

            _context.ProjectMasters.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}