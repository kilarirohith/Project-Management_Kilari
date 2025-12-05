// server/Controllers/TaskController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using server.Authorization;         
using server.DTOs;
using server.Services.Interfaces;
using server.Models;   
using System.Linq;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   
    [Authorize]                   
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // ---------- helper mapper ----------
        private static TaskDTO MapToDto(ProjectTask t)
        {
            return new TaskDTO
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,

                ProjectId = t.ProjectId,
                ProjectName = t.Project?.ProjectName ?? string.Empty,

                AssignedToUserId = t.AssignedToUserId,
                AssignedUserName = t.AssignedToUser?.FullName,

                CreatedAt = t.CreatedAt,
                DueDate = t.DueDate,
                ActualClosureDate = t.ActualClosureDate,

                Type = t.Type,
                ProduceStep = t.ProduceStep,
                SampleData = t.SampleData,
                AcceptanceCriteria = t.AcceptanceCriteria,
                TestingStatus = t.TestingStatus,
                TestingDoneBy = t.TestingDoneBy
            };
        }

        // ---------- GET: api/Task ----------
        [HttpGet]
        [PermissionAuthorize("Task Tracker", "Read")]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _taskService.GetAllAsync();
            var dtos = tasks.Select(MapToDto);
            return Ok(dtos);
        }

        // ---------- GET: api/Task/{id} ----------
        [HttpGet("{id:int}")]
        [PermissionAuthorize("Task Tracker", "Read")]
        public async Task<IActionResult> GetTask(int id)
        {
            var t = await _taskService.GetByIdAsync(id);
            if (t == null) return NotFound("Task not found");

            return Ok(MapToDto(t));
        }

        // ---------- POST: api/Task ----------
        [HttpPost]
        [PermissionAuthorize("Task Tracker", "Create")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _taskService.CreateAsync(dto);
            var resultDto = MapToDto(created);

            return CreatedAtAction(nameof(GetTask), new { id = resultDto.Id }, resultDto);
        }

        // ---------- PUT: api/Task/{id} ----------
        [HttpPut("{id:int}")]
        [PermissionAuthorize("Task Tracker", "Update")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] CreateTaskDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _taskService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound("Task not found");

            return Ok(MapToDto(updated));
        }

        // ---------- DELETE: api/Task/{id} ----------
        [HttpDelete("{id:int}")]
        [PermissionAuthorize("Task Tracker", "Delete")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var ok = await _taskService.DeleteAsync(id);
            if (!ok)
                return NotFound("Task not found");

            return NoContent();
        }
    }
}
