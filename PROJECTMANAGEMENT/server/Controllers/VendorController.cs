using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetVendors()
        {
            var vendors = await _vendorService.GetAllAsync();
            return Ok(vendors);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetVendor(int id)
        {
            var vendor = await _vendorService.GetByIdAsync(id);
            if (vendor == null)
                return NotFound("Vendor not found");
            return Ok(vendor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateVendor([FromBody] CreateVendorDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _vendorService.CreateAsync(dto);
            return Ok(new { message = "Vendor created successfully", vendor = created });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateVendor(int id, [FromBody] CreateVendorDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _vendorService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound("Vendor not found");

            return Ok(new { message = "Vendor updated successfully", vendor = updated });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var success = await _vendorService.DeleteAsync(id);
            if (!success)
                return NotFound("Vendor not found");

            return Ok(new { message = "Vendor deleted successfully" });
        }
    }
}
