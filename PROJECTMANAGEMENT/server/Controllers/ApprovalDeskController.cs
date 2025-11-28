using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Authorization;                 // 👈 IMPORTANT
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]                              // 👈 must be logged in
    public class ApprovalDeskController : ControllerBase
    {
        private readonly IApprovalDeskService _service;

        public ApprovalDeskController(IApprovalDeskService service)
        {
            _service = service;
        }

        // ---------- GET: api/ApprovalDesk ----------
        [HttpGet]
        [PermissionAuthorize("Projects", "Read")]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        // ---------- GET: api/ApprovalDesk/{id} ----------
        [HttpGet("{id}")]
        [PermissionAuthorize("Projects", "Read")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        // ---------- POST: api/ApprovalDesk ----------
        [HttpPost]
        [PermissionAuthorize("Projects", "Create")]
        public async Task<IActionResult> Create([FromBody] CreateApprovalDeskDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        // ---------- PUT: api/ApprovalDesk/{id} ----------
        [HttpPut("{id}")]
        [PermissionAuthorize("Projects", "Update")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateApprovalDeskDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        // ---------- DELETE: api/ApprovalDesk/{id} ----------
        [HttpDelete("{id}")]
        [PermissionAuthorize("Projects", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
