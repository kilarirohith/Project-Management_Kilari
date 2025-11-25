using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ Require auth so we always have a user
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _ticketService.GetAllAsync();
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null) return NotFound(new { message = "Ticket not found" });
            return Ok(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // ✅ Get user id from JWT (we set this as ClaimTypes.NameIdentifier in AuthHelper)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "User id not found in token" });
            }

            // ✅ Force CreatedByUserId from token, ignore whatever frontend sends
            dto.CreatedByUserId = int.Parse(userIdClaim);

            // Optional: auto-fill RaisedBy from username/email if empty
            if (string.IsNullOrWhiteSpace(dto.RaisedBy))
            {
                dto.RaisedBy = User.Identity?.Name ?? "Unknown";
            }

            // Optional: set TimeRaised if client did not
            if (string.IsNullOrWhiteSpace(dto.TimeRaised))
            {
                dto.TimeRaised = DateTime.Now.ToString("HH:mm");
            }

            var created = await _ticketService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateTicketDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // We do NOT change CreatedByUserId on update
            var updated = await _ticketService.UpdateAsync(id, dto);
            if (updated == null) return NotFound(new { message = "Ticket not found" });

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _ticketService.DeleteAsync(id);
            if (!ok) return NotFound(new { message = "Ticket not found" });

            return NoContent();
        }
    }
}
