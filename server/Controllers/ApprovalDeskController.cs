
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
    public class ApprovalDeskController : ControllerBase
    {
        private readonly IApprovalDeskService _service;

        public ApprovalDeskController(IApprovalDeskService service)
        {
            _service = service;
        }

        [HttpGet]
        [PermissionAuthorize("Approval Desk", "Read")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status,
            [FromQuery] string? vendorName,
            [FromQuery] string? projectName,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (items, totalCount) = await _service.GetPagedAsync(
                status,
                vendorName,
                projectName,
                dateFrom,
                dateTo,
                page,
                pageSize);

            return Ok(new { items, totalCount });
        }

        [HttpGet("{id}")]
        [PermissionAuthorize("Approval Desk", "Read")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        [PermissionAuthorize("Approval Desk", "Create")]
        public async Task<IActionResult> Create([FromBody] CreateApprovalDeskDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        [PermissionAuthorize("Approval Desk", "Update")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateApprovalDeskDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        [PermissionAuthorize("Approval Desk", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
