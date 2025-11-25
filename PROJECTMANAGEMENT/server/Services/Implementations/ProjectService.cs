using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectDTO>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.Client)
                .OrderByDescending(p => p.Id)
                .Select(p => new ProjectDTO
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    ProjectCode = p.ProjectCode,
                    ProjectType = p.ProjectType,
                    ClientId = p.ClientId,
                    ClientName = p.Client.ClientName,
                    ClientLocation = p.ClientLocation,
                    Unit = p.Unit,
                    Milestone = p.Milestone,
                    PlanStartDate = p.PlanStartDate,
                    PlanEndDate = p.PlanEndDate,
                    ActualStartDate = p.ActualStartDate,
                    ActualEndDate = p.ActualEndDate,
                    Status = p.Status
                })
                .ToListAsync();
        }

        public async Task<ProjectDTO?> GetByIdAsync(int id)
        {
            var p = await _context.Projects
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null) return null;

            return new ProjectDTO
            {
                Id = p.Id,
                ProjectName = p.ProjectName,
                ProjectCode = p.ProjectCode,
                ProjectType = p.ProjectType,
                ClientId = p.ClientId,
                ClientName = p.Client.ClientName,
                ClientLocation = p.ClientLocation,
                Unit = p.Unit,
                Milestone = p.Milestone,
                PlanStartDate = p.PlanStartDate,
                PlanEndDate = p.PlanEndDate,
                ActualStartDate = p.ActualStartDate,
                ActualEndDate = p.ActualEndDate,
                Status = p.Status
            };
        }

        public async Task<ProjectDTO> CreateAsync(CreateProjectDTO dto)
        {
            var client = await _context.Clients.FindAsync(dto.ClientId);
            if (client == null) throw new Exception("Invalid Client ID");

            var project = new Project
            {
                ProjectName = dto.ProjectName,
                ProjectCode = string.IsNullOrWhiteSpace(dto.ProjectCode)
                    ? GenerateCode()
                    : dto.ProjectCode,
                ProjectType = dto.ProjectType,
                ClientId = dto.ClientId,
                ClientLocation = dto.ClientLocation,
                Unit = dto.Unit,
                Milestone = dto.Milestone,
                PlanStartDate = dto.PlanStartDate,
                PlanEndDate = dto.PlanEndDate,
                ActualStartDate = dto.ActualStartDate,
                ActualEndDate = dto.ActualEndDate,
                Status = dto.Status ?? "Running"
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return new ProjectDTO
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ProjectCode = project.ProjectCode,
                ProjectType = project.ProjectType,
                ClientId = project.ClientId,
                ClientName = client.ClientName,
                ClientLocation = project.ClientLocation,
                Unit = project.Unit,
                Milestone = project.Milestone,
                PlanStartDate = project.PlanStartDate,
                PlanEndDate = project.PlanEndDate,
                ActualStartDate = project.ActualStartDate,
                ActualEndDate = project.ActualEndDate,
                Status = project.Status
            };
        }

        public async Task<ProjectDTO?> UpdateAsync(int id, CreateProjectDTO dto)
        {
            var project = await _context.Projects
                .Include(p => p.Client)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return null;

            var client = await _context.Clients.FindAsync(dto.ClientId);
            if (client == null) throw new Exception("Invalid Client ID");

            project.ProjectName = dto.ProjectName ?? project.ProjectName;
            project.ProjectCode = string.IsNullOrWhiteSpace(dto.ProjectCode)
                ? project.ProjectCode
                : dto.ProjectCode;
            project.ProjectType = dto.ProjectType ?? project.ProjectType;
            project.ClientId = dto.ClientId;
            project.ClientLocation = dto.ClientLocation ?? project.ClientLocation;
            project.Unit = dto.Unit ?? project.Unit;
            project.Milestone = dto.Milestone ?? project.Milestone;
            project.PlanStartDate = dto.PlanStartDate ?? project.PlanStartDate;
            project.PlanEndDate = dto.PlanEndDate ?? project.PlanEndDate;
            project.ActualStartDate = dto.ActualStartDate ?? project.ActualStartDate;
            project.ActualEndDate = dto.ActualEndDate ?? project.ActualEndDate;
            project.Status = dto.Status ?? project.Status;

            await _context.SaveChangesAsync();

            return new ProjectDTO
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ProjectCode = project.ProjectCode,
                ProjectType = project.ProjectType,
                ClientId = project.ClientId,
                ClientName = client.ClientName,
                ClientLocation = project.ClientLocation,
                Unit = project.Unit,
                Milestone = project.Milestone,
                PlanStartDate = project.PlanStartDate,
                PlanEndDate = project.PlanEndDate,
                ActualStartDate = project.ActualStartDate,
                ActualEndDate = project.ActualEndDate,
                Status = project.Status
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateCode()
        {
            var lastId = _context.Projects.Any()
                ? _context.Projects.Max(p => p.Id) + 1
                : 1;
            return $"PRJ-{lastId:000}";
        }
    }
}
