
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Authorization;             
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]                          
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // ---------- GET: api/Project ----------
        [HttpGet]
        [PermissionAuthorize("Projects", "Read")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _projectService.GetAllAsync();
            return Ok(items);
        }

        // ---------- GET: api/Project/{id} ----------
        [HttpGet("{id}")]
        [PermissionAuthorize("Projects", "Read")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _projectService.GetByIdAsync(id);
            return project == null
                ? NotFound(new { message = "Project not found" })
                : Ok(project);
        }

        // ---------- POST: api/Project ----------
        [HttpPost]
        [PermissionAuthorize("Projects", "Create")]
        public async Task<IActionResult> Create([FromBody] CreateProjectDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _projectService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ---------- PUT: api/Project/{id} ----------
        [HttpPut("{id}")]
        [PermissionAuthorize("Projects", "Update")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updated = await _projectService.UpdateAsync(id, dto);
                return updated == null
                    ? NotFound(new { message = "Project not found" })
                    : Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ---------- DELETE: api/Project/{id} ----------
        [HttpDelete("{id}")]
        [PermissionAuthorize("Projects", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _projectService.DeleteAsync(id);
            return ok
                ? NoContent()
                : NotFound(new { message = "Project not found" });
        }
    }
}
