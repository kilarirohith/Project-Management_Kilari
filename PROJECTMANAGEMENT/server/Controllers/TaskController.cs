using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ITaskTrackerService _taskTrackerService;

        public TaskController(ITaskService taskService, ITaskTrackerService taskTrackerService)
        {
            _taskService = taskService;
            _taskTrackerService = taskTrackerService;
        }

        // GET: api/task
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _taskService.GetAllAsync();
            var trackers = await _taskTrackerService.GetAllAsync();

            var trackerDict = trackers.ToDictionary(t => t.TaskId, t => t);

            var dtos = tasks.Select(t =>
            {
                trackerDict.TryGetValue(t.Id, out var tracker);
                return new TaskDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.ProjectName,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedUserName = t.AssignedToUser != null ? t.AssignedToUser.FullName : null,
                    CreatedAt = t.CreatedAt,
                    DueDate = t.DueDate,
                    Progress = tracker?.Progress
                };
            });

            return Ok(dtos);
        }

        // GET: api/task/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var t = await _taskService.GetByIdAsync(id);
            if (t == null) return NotFound("Task not found");

            var trackers = await _taskTrackerService.GetAllAsync();
            var tracker = trackers.FirstOrDefault(x => x.TaskId == t.Id);

            var dto = new TaskDTO
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.ProjectName,
                AssignedToUserId = t.AssignedToUserId,
                AssignedUserName = t.AssignedToUser?.FullName,
                CreatedAt = t.CreatedAt,
                DueDate = t.DueDate,
                Progress = tracker?.Progress
            };

            return Ok(dto);
        }

        // POST: api/task
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var task = await _taskService.CreateAsync(dto);
            return Ok(new { message = "Task created successfully", taskId = task.Id });
        }

        // PUT: api/task/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] CreateTaskDTO dto)
        {
            var updated = await _taskService.UpdateAsync(id, dto);
            if (updated == null) return NotFound("Task not found");

            return Ok(new { message = "Task updated successfully" });
        }

        // DELETE: api/task/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var ok = await _taskService.DeleteAsync(id);
            if (!ok) return NotFound("Task not found");

            return Ok(new { message = "Task deleted successfully" });
        }
    }
}
