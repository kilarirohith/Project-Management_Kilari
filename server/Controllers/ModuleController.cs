using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Authorization;
using server.Data;
using server.Models;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class ModuleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModuleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [PermissionAuthorize("Masters", "Read")]
        public async Task<ActionResult<IEnumerable<AppModule>>> GetAll()
        {
            var modules = await _context.AppModules
                .OrderBy(m => m.Name)
                .ToListAsync();
            return Ok(modules);
        }

        [HttpPost]
        [PermissionAuthorize("Masters", "Create")]
        public async Task<ActionResult<AppModule>> Create([FromBody] AppModule dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Module name is required");

            var exists = await _context.AppModules
                .AnyAsync(m => m.Name == dto.Name);
            if (exists)
                return BadRequest("Module already exists");

            var module = new AppModule { Name = dto.Name.Trim() };
            _context.AppModules.Add(module);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = module.Id }, module);
        }

        [HttpDelete("{id}")]
        [PermissionAuthorize("Masters", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var module = await _context.AppModules.FindAsync(id);
            if (module == null) return NotFound();

            bool used = await _context.RolePermissions
                .AnyAsync(p => p.Module == module.Name);
            if (used)
                return BadRequest("Module is assigned in one or more roles and cannot be deleted.");

            _context.AppModules.Remove(module);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
