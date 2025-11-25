using Microsoft.AspNetCore.Authorization; // <--- Add this namespace
using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Option A: Protect the ENTIRE controller (Only Admins can even see roles)
    // [Authorize(Roles = "Admin")] 
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // Allow everyone (or just logged in users) to VIEW roles, 
        // but only Admin can modify them.
        [HttpGet]
        [Authorize] // Any logged-in user can view
        public async Task<IActionResult> GetAll() => Ok(await _roleService.GetAllAsync());

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            return role == null ? NotFound() : Ok(role);
        }

        // --- RESTRICT THESE TO ADMIN ONLY ---

        [HttpPost]
        [Authorize(Roles = "Admin")] // <--- Only Admin can Create
        public async Task<IActionResult> Create([FromBody] RoleDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _roleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // <--- Only Admin can Update
        public async Task<IActionResult> Update(int id, [FromBody] RoleDTO dto)
        {
            var updated = await _roleService.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // <--- Only Admin can Delete
        public async Task<IActionResult> Delete(int id)
        {
            return await _roleService.DeleteAsync(id) ? NoContent() : NotFound();
        }
    }
}