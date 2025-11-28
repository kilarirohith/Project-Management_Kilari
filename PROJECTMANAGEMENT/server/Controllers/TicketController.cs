using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Authorization;
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        [PermissionAuthorize("Ticket Tracker", "Read")]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetAll()
        {
            var tickets = await _ticketService.GetAllAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        [PermissionAuthorize("Ticket Tracker", "Read")]
        public async Task<ActionResult<TicketDTO>> GetById(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        [HttpPost]
        [PermissionAuthorize("Ticket Tracker", "Create")]
        public async Task<ActionResult<TicketDTO>> Create([FromBody] CreateTicketDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User id not found in token.");

            int currentUserId = int.Parse(userIdClaim);

            var created = await _ticketService.CreateAsync(dto, currentUserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [PermissionAuthorize("Ticket Tracker", "Update")]
        public async Task<ActionResult<TicketDTO>> Update(int id, [FromBody] CreateTicketDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User id not found in token.");

            int currentUserId = int.Parse(userIdClaim);

            var updated = await _ticketService.UpdateAsync(id, dto, currentUserId);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [PermissionAuthorize("Ticket Tracker", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _ticketService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
