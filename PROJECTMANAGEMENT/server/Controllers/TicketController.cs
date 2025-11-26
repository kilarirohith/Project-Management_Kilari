using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // if you want auth, otherwise you can comment this temporarily
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: api/Ticket
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetAll()
        {
            var tickets = await _ticketService.GetAllAsync();
            return Ok(tickets);
        }

        // GET: api/Ticket/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDTO>> GetById(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        // POST: api/Ticket
// POST: api/Ticket
[HttpPost]
public async Task<ActionResult<TicketDTO>> Create([FromBody] CreateTicketDTO dto)
{
    // ✅ Get user id from JWT
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userIdClaim))
        return Unauthorized("User id not found in token.");

    int currentUserId = int.Parse(userIdClaim);

    var created = await _ticketService.CreateAsync(dto, currentUserId);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
}

// PUT: api/Ticket/5
[HttpPut("{id}")]
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


        // DELETE: api/Ticket/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _ticketService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
