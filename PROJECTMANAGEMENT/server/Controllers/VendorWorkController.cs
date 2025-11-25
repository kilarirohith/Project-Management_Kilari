using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorWorkController : ControllerBase
    {
        private readonly IVendorWorkService _vendorWorkService;

        public VendorWorkController(IVendorWorkService vendorWorkService)
        {
            _vendorWorkService = vendorWorkService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var vendorWorks = await _vendorWorkService.GetAllAsync();
            return Ok(vendorWorks);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var vendorWork = await _vendorWorkService.GetByIdAsync(id);
            if (vendorWork is null)
                return NotFound(new { message = "Vendor Work not found" });

            return Ok(vendorWork);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateVendorWorkDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _vendorWorkService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateVendorWorkDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _vendorWorkService.UpdateAsync(id, dto);
            if (updated is null)
                return NotFound(new { message = "Vendor Work not found" });

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _vendorWorkService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = "Vendor Work not found" });

            return NoContent();
        }
    }
}
