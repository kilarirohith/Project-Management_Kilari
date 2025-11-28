using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Authorization;                 // 👈 for PermissionAuthorize
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]                              // 👈 must be logged in
    public class VendorWorkController : ControllerBase
    {
        private readonly IVendorWorkService _vendorWorkService;

        public VendorWorkController(IVendorWorkService vendorWorkService)
        {
            _vendorWorkService = vendorWorkService;
        }

        // ---------- GET: api/VendorWork ----------
        [HttpGet]
        [PermissionAuthorize("Projects", "Read")]
        public async Task<IActionResult> GetAll()
        {
            var vendorWorks = await _vendorWorkService.GetAllAsync();
            return Ok(vendorWorks);
        }

        // ---------- GET: api/VendorWork/{id} ----------
        [HttpGet("{id}")]
        [PermissionAuthorize("Projects", "Read")]
        public async Task<IActionResult> GetById(int id)
        {
            var vendorWork = await _vendorWorkService.GetByIdAsync(id);
            if (vendorWork is null)
                return NotFound(new { message = "Vendor Work not found" });

            return Ok(vendorWork);
        }

        // ---------- POST: api/VendorWork ----------
        [HttpPost]
        [PermissionAuthorize("Projects", "Create")]
        public async Task<IActionResult> Create([FromBody] CreateVendorWorkDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _vendorWorkService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ---------- PUT: api/VendorWork/{id} ----------
        [HttpPut("{id}")]
        [PermissionAuthorize("Projects", "Update")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateVendorWorkDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _vendorWorkService.UpdateAsync(id, dto);
            if (updated is null)
                return NotFound(new { message = "Vendor Work not found" });

            return Ok(updated);
        }

        // ---------- DELETE: api/VendorWork/{id} ----------
        [HttpDelete("{id}")]
        [PermissionAuthorize("Projects", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _vendorWorkService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = "Vendor Work not found" });

            return NoContent();
        }
    }
}
