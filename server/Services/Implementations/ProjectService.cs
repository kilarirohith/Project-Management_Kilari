// server/Services/Implementations/ProjectService.cs
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

       

private static ProjectDTO ToDto(Project p)
{
    return new ProjectDTO
    {
        Id = p.Id,
        ProjectCode = p.ProjectCode,
        ProjectName = p.ProjectName,
        ProjectType = p.ProjectType,
        ClientName = p.Client.ClientName,
        ClientLocation = p.ClientLocation,
        Unit = p.Unit,
        Milestone = p.Milestone,
        PlanStartDate = p.PlanStartDate,
        PlanEndDate = p.PlanEndDate,
        ActualStartDate = p.ActualStartDate,
        ActualEndDate = p.ActualEndDate,
        ElapsedDays = p.ElapsedDays,
        Status = p.Status          // 👈 important
    };
}

public async Task<ProjectDTO> CreateAsync(CreateProjectDTO dto)
{
    var project = new Project
    {
        ProjectCode = dto.ProjectCode,
        ProjectName = dto.ProjectName,
        ProjectType = dto.ProjectType,
        ClientId = dto.ClientId,
        ClientLocation = dto.ClientLocation,
        Unit = dto.Unit,
        Milestone = dto.Milestone,
        PlanStartDate = dto.PlanStartDate,
        PlanEndDate = dto.PlanEndDate,
        Status = dto.Status        // 👈 manual status from UI
    };

    _context.Projects.Add(project);
    await _context.SaveChangesAsync();

    await _context.Entry(project).Reference(p => p.Client).LoadAsync();
    return ToDto(project);
}

public async Task<ProjectDTO?> UpdateAsync(int id, UpdateProjectDTO dto)
{
    var project = await _context.Projects.Include(p => p.Client)
                                         .FirstOrDefaultAsync(p => p.Id == id);
    if (project == null) return null;

    project.ProjectCode = dto.ProjectCode;
    project.ProjectName = dto.ProjectName;
    project.ProjectType = dto.ProjectType;
    project.ClientId = dto.ClientId;
    project.ClientLocation = dto.ClientLocation;
    project.Unit = dto.Unit;
    project.Milestone = dto.Milestone;
    project.PlanStartDate = dto.PlanStartDate;
    project.PlanEndDate = dto.PlanEndDate;
    project.ActualStartDate = dto.ActualStartDate;
    project.ActualEndDate = dto.ActualEndDate;
    project.ElapsedDays = dto.ElapsedDays;
    project.Status = dto.Status;      // 👈 update from UI

    await _context.SaveChangesAsync();
    return ToDto(project);
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
