using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class TaskTrackerService : ITaskTrackerService
    {
        private readonly AppDbContext _context;

        public TaskTrackerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskTracker>> GetAllAsync()
        {
            return await _context.TaskTrackers
                .Include(t => t.Task)
                .ToListAsync();
        }

        public async Task<TaskTracker> CreateOrUpdateAsync(TaskTrackerDTO dto)
        {
            var existing = await _context.TaskTrackers
                .FirstOrDefaultAsync(x => x.TaskId == dto.TaskId);

            if (existing == null)
            {
                var tracker = new TaskTracker
                {
                    TaskId = dto.TaskId,
                    Progress = dto.Progress,
                    Remarks = dto.Remarks,
                    UpdatedAt = DateTime.Now
                };

                _context.TaskTrackers.Add(tracker);
                await _context.SaveChangesAsync();
                return tracker;
            }
            else
            {
                existing.Progress = dto.Progress;
                existing.Remarks = dto.Remarks;
                existing.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return existing;
            }
        }
    }
}
