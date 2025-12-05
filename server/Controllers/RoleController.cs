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
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // ---------- READ ROLES ----------
        [HttpGet]
        [PermissionAuthorize("Masters", "Read")]
        public async Task<IActionResult> GetAll() => Ok(await _roleService.GetAllAsync());

        [HttpGet("{id}")]
        [PermissionAuthorize("Masters", "Read")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            return role == null ? NotFound() : Ok(role);
        }

        // ---------- CREATE ROLE ----------
        [HttpPost]
        [PermissionAuthorize("Masters", "Create")]
        public async Task<IActionResult> Create([FromBody] RoleDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _roleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ---------- UPDATE ROLE ----------
        [HttpPut("{id}")]
        [PermissionAuthorize("Masters", "Update")]
        public async Task<IActionResult> Update(int id, [FromBody] RoleDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _roleService.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        // ---------- DELETE ROLE ----------
        [HttpDelete("{id}")]
        [PermissionAuthorize("Masters", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _roleService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
