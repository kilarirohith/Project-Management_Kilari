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
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        [HttpGet]
        [PermissionAuthorize("Masters", "Read")]
        
        public async Task<IActionResult> GetVendors()
        {
            var vendors = await _vendorService.GetAllAsync();
            return Ok(vendors);
        }

        [HttpGet("{id}")]
        [PermissionAuthorize("Masters", "Read")]
        
        public async Task<IActionResult> GetVendor(int id)
        {
            var vendor = await _vendorService.GetByIdAsync(id);
            if (vendor == null)
                return NotFound("Vendor not found");
            return Ok(vendor);
        }

        [HttpPost]
        [PermissionAuthorize("Masters", "Create")]
        
        public async Task<IActionResult> CreateVendor([FromBody] CreateVendorDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _vendorService.CreateAsync(dto);
            return Ok(new { message = "Vendor created successfully", vendor = created });
        }

        [HttpPut("{id}")]
        [PermissionAuthorize("Masters", "Update")]
        
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
        [PermissionAuthorize("Masters", "Delete")]
        
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var success = await _vendorService.DeleteAsync(id);
            if (!success)
                return NotFound("Vendor not found");

            return Ok(new { message = "Vendor deleted successfully" });
        }
    }
}
