using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskTrackerController : ControllerBase
    {
        private readonly ITaskTrackerService _taskTrackerService;

        public TaskTrackerController(ITaskTrackerService taskTrackerService)
        {
            _taskTrackerService = taskTrackerService;
        }

        // POST: api/tasktracker
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] TaskTrackerDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tracker = await _taskTrackerService.CreateOrUpdateAsync(dto);
            return Ok(new { message = "Progress updated", tracker });
        }
    }
}
