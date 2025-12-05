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
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        [PermissionAuthorize("Masters", "Read")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _clientService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [PermissionAuthorize("Masters", "Read")]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            return client == null ? NotFound() : Ok(client);
        }

        [HttpGet("{clientId}/locations")]
        [PermissionAuthorize("Masters", "Read")]
        public async Task<IActionResult> GetLocationsByClient(int clientId)
        {
            var locations = await _clientService.GetLocationsByClientAsync(clientId);
            if (locations.Count == 0) return NotFound(new { message = "Client not found or no locations" });
            return Ok(locations);
        }

        [HttpPost]
        [PermissionAuthorize("Masters", "Create")]
        public async Task<IActionResult> Create([FromBody] ClientDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _clientService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [PermissionAuthorize("Masters", "Update")]
        public async Task<IActionResult> Update(int id, [FromBody] ClientDTO dto)
        {
            var updated = await _clientService.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        [PermissionAuthorize("Masters", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _clientService.DeleteAsync(id) ? NoContent() : NotFound();
        }
    }
}
