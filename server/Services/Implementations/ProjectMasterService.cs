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

        public ProjectMasterService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectMasterDTO>> GetAllAsync()
        {
            return await _context.ProjectMasters
                .Select(p => new ProjectMasterDTO
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    Description = p.Description
                })
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<ProjectMasterDTO?> GetByIdAsync(int id)
        {
            var project = await _context.ProjectMasters
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return null;

            return new ProjectMasterDTO
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                Description = project.Description
            };
        }

        public async Task<ProjectMasterDTO> CreateAsync(CreateProjectMasterDTO dto)
        {
            var projectMaster = new ProjectMaster
            {
                ProjectName = dto.ProjectName,
                Description = dto.Description
            };

            _context.ProjectMasters.Add(projectMaster);
            await _context.SaveChangesAsync();

            return new ProjectMasterDTO
            {
                Id = projectMaster.Id,
                ProjectName = projectMaster.ProjectName,
                Description = projectMaster.Description
            };
        }

        public async Task<ProjectMasterDTO?> UpdateAsync(int id, CreateProjectMasterDTO dto)
        {
            var project = await _context.ProjectMasters.FindAsync(id);
            if (project == null) return null;

            project.ProjectName = dto.ProjectName;
            project.Description = dto.Description;

            await _context.SaveChangesAsync();

            return new ProjectMasterDTO
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                Description = project.Description
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
