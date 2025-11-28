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
    public class MilestoneMasterController : ControllerBase
    {
        private readonly IMilestoneMasterService _service;

        public MilestoneMasterController(IMilestoneMasterService service)
        {
            _service = service;
        }

        [HttpGet]
        [PermissionAuthorize("Masters", "Read")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        [PermissionAuthorize("Masters", "Create")]
        public async Task<IActionResult> Create([FromBody] MilestoneMasterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _service.CreateAsync(dto));
        }

        [HttpPut("{id}")]
        [PermissionAuthorize("Masters", "Update")]
        public async Task<IActionResult> Update(int id, [FromBody] MilestoneMasterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var res = await _service.UpdateAsync(id, dto);
            return res == null ? NotFound() : Ok(res);
        }

        [HttpDelete("{id}")]
        [PermissionAuthorize("Masters", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _service.DeleteAsync(id) ? NoContent() : NotFound();
        }
    }
}
