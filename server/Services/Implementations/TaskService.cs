
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectTask>> GetAllAsync()
        {
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .ToListAsync();
        }

        public async Task<ProjectTask?> GetByIdAsync(int id)
        {
            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<ProjectTask> CreateAsync(CreateTaskDTO dto)
        {
            var task = new ProjectTask
            {
                Title = dto.Title,
                Description = dto.Description ?? string.Empty,
                Priority = dto.Priority ?? "Normal",
                Status = dto.Status ?? "Open",
                ProjectId = dto.ProjectId,
                AssignedToUserId = dto.AssignedToUserId,
                DueDate = dto.DueDate,
                ActualClosureDate = dto.ActualClosureDate,
                Type = dto.Type,
                ProduceStep = dto.ProduceStep,
                SampleData = dto.SampleData,
                AcceptanceCriteria = dto.AcceptanceCriteria,
                TestingStatus = dto.TestingStatus,
                TestingDoneBy = dto.TestingDoneBy
            };

            _context.ProjectTasks.Add(task);
            await _context.SaveChangesAsync();

          
            await _context.Entry(task).Reference(t => t.Project).LoadAsync();
            await _context.Entry(task).Reference(t => t.AssignedToUser).LoadAsync();

            return task;
        }

        public async Task<ProjectTask?> UpdateAsync(int id, CreateTaskDTO dto)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return null;

            task.Title = dto.Title ?? task.Title;
            task.Description = dto.Description ?? task.Description;
            task.Priority = dto.Priority ?? task.Priority;
            task.Status = dto.Status ?? task.Status;
            task.ProjectId = dto.ProjectId;
            task.AssignedToUserId = dto.AssignedToUserId;
            task.DueDate = dto.DueDate ?? task.DueDate;
            task.ActualClosureDate = dto.ActualClosureDate ?? task.ActualClosureDate;

            task.Type = dto.Type ?? task.Type;
            task.ProduceStep = dto.ProduceStep ?? task.ProduceStep;
            task.SampleData = dto.SampleData ?? task.SampleData;
            task.AcceptanceCriteria = dto.AcceptanceCriteria ?? task.AcceptanceCriteria;
            task.TestingStatus = dto.TestingStatus ?? task.TestingStatus;
            task.TestingDoneBy = dto.TestingDoneBy ?? task.TestingDoneBy;

            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null) return false;

            _context.ProjectTasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
