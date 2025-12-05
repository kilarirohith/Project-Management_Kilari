using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Authorization; 
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class ProjectMasterController : ControllerBase
{
    private readonly IProjectMasterService _projectService;

    public ProjectMasterController(IProjectMasterService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _projectService.GetAllAsync());
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        return project == null ? NotFound() : Ok(project);
    }

    [HttpPost]
    [PermissionAuthorize("Projects", "Create")]
    public async Task<IActionResult> Create([FromBody] CreateProjectMasterDTO dto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);
      try 
      {
          var created = await _projectService.CreateAsync(dto);
          return Ok(created);
      }
      catch (Exception ex)
      {
          return BadRequest(new { message = ex.Message });
      }
    }

    [HttpPut("{id}")]
    [PermissionAuthorize("Projects", "Update")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateProjectMasterDTO dto)
    {
        try
        {
            var updated = await _projectService.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [PermissionAuthorize("Projects", "Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _projectService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}

}
