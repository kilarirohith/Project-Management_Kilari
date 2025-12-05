using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Authorization;     
using server.Data;
using server.DTOs;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]                  
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Dashboard/summary
        [HttpGet("summary")]
        [PermissionAuthorize("Dashboard", "Read")]   
        [Authorize]   
        public async Task<IActionResult> GetSummary()
        {
            // ---------- PROJECTS ----------
            var totalProjects = await _context.Projects.CountAsync();
            var runningProjects = await _context.Projects.CountAsync(p => p.Status == "Running");
            var completedProjects = await _context.Projects.CountAsync(p => p.Status == "Completed");
            var delayedProjects = await _context.Projects.CountAsync(p => p.Status == "Delayed");
            var onHoldProjects = await _context.Projects.CountAsync(p => p.Status == "OnHold");

            // ---------- TASKS ----------
            var totalTasks = await _context.ProjectTasks.CountAsync();
            var completedTasks = await _context.ProjectTasks.CountAsync(t => t.Status == "Completed");
            var pendingTasks = totalTasks - completedTasks;

            // ---------- TICKETS ----------
            var totalTickets = await _context.Tickets.CountAsync();
            var openTickets = await _context.Tickets.CountAsync(t => t.Status == "Open");
            var closedTickets = await _context.Tickets.CountAsync(t => t.Status == "Closed");
            var onHoldTickets = await _context.Tickets.CountAsync(t => t.Status == "On Hold");

            var summary = new DashboardSummaryDTO
            {
                TotalProjects = totalProjects,
                RunningProjects = runningProjects,
                CompletedProjects = completedProjects,
                DelayedProjects = delayedProjects,
                OnHoldProjects = onHoldProjects,

                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                PendingTasks = pendingTasks,

                TotalTickets = totalTickets,
                OpenTickets = openTickets,
                ClosedTickets = closedTickets,
                OnHoldTickets = onHoldTickets
            };

            return Ok(summary);
        }
    }
}
