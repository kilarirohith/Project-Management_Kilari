using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;

        public TicketService(AppDbContext context)
        {
            _context = context;
        }

        // -------------------------
        // Get all tickets as DTOs
        // -------------------------
        public async Task<List<TicketDTO>> GetAllAsync()
        {
            return await _context.Tickets
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .OrderByDescending(t => t.Id)
                .Select(t => new TicketDTO
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Title = t.Title,
                    Description = t.Description,
                    ClientName = t.ClientName,
                    Location = t.Location,
                    Category = t.Category,
                    RaisedBy = t.RaisedBy,
                    AssignedTo = t.AssignedTo,
                    Priority = t.Priority,
                    Status = t.Status,
                    Resolution = t.Resolution,
                    DateRaised = t.DateRaised,
                    TimeRaised = t.TimeRaised,
                    CreatedByName = t.CreatedByUser != null ? t.CreatedByUser.Username : string.Empty,
                    AssignedToName = t.AssignedToUser != null ? t.AssignedToUser.Username : string.Empty,
                    CreatedByUserId = t.CreatedByUserId,
                    AssignedToUserId = t.AssignedToUserId
                })
                .ToListAsync();
        }

        // -------------------------
        // Get single ticket
        // -------------------------
        public async Task<TicketDTO?> GetByIdAsync(int id)
        {
            var t = await _context.Tickets
                .Include(x => x.CreatedByUser)
                .Include(x => x.AssignedToUser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (t == null) return null;

            return new TicketDTO
            {
                Id = t.Id,
                TicketNumber = t.TicketNumber,
                Title = t.Title,
                Description = t.Description,
                ClientName = t.ClientName,
                Location = t.Location,
                Category = t.Category,
                RaisedBy = t.RaisedBy,
                AssignedTo = t.AssignedTo,
                Priority = t.Priority,
                Status = t.Status,
                Resolution = t.Resolution,
                DateRaised = t.DateRaised,
                TimeRaised = t.TimeRaised,
                CreatedByName = t.CreatedByUser != null ? t.CreatedByUser.Username : string.Empty,
                AssignedToName = t.AssignedToUser != null ? t.AssignedToUser.Username : string.Empty,
                CreatedByUserId = t.CreatedByUserId,
                AssignedToUserId = t.AssignedToUserId
            };
        }

        // -------------------------
        // Create new ticket
        // -------------------------
        public async Task<TicketDTO> CreateAsync(CreateTicketDTO dto)
        {
            var count = await _context.Tickets.CountAsync() + 1;
            var ticketNum = $"TKT-{count:D3}";

            var ticket = new Ticket
            {
                TicketNumber = ticketNum,
                Title = dto.Title ?? "No Title",
                Description = dto.Description,
                ClientName = dto.ClientName,
                Location = dto.Location,
                Category = dto.Category,
                RaisedBy = dto.RaisedBy,
                AssignedTo = dto.AssignedTo,
                Priority = dto.Priority,
                Status = dto.Status,
                Resolution = dto.Resolution,
                TimeRaised = dto.TimeRaised,
                DateRaised = DateTime.Now,
                CreatedByUserId = dto.CreatedByUserId,
                AssignedToUserId = dto.AssignedToUserId
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            ticket = await _context.Tickets
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .FirstAsync(t => t.Id == ticket.Id);

            return new TicketDTO
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                Description = ticket.Description,
                ClientName = ticket.ClientName,
                Location = ticket.Location,
                Category = ticket.Category,
                RaisedBy = ticket.RaisedBy,
                AssignedTo = ticket.AssignedTo,
                Priority = ticket.Priority,
                Status = ticket.Status,
                Resolution = ticket.Resolution,
                DateRaised = ticket.DateRaised,
                TimeRaised = ticket.TimeRaised,
                CreatedByName = ticket.CreatedByUser != null ? ticket.CreatedByUser.Username : string.Empty,
                AssignedToName = ticket.AssignedToUser != null ? ticket.AssignedToUser.Username : string.Empty,
                CreatedByUserId = ticket.CreatedByUserId,
                AssignedToUserId = ticket.AssignedToUserId
            };
        }

        // -------------------------
        // Update existing ticket
        // -------------------------
        public async Task<TicketDTO?> UpdateAsync(int id, CreateTicketDTO dto)
        {
            var ticket = await _context.Tickets
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return null;

            ticket.Title = dto.Title;
            ticket.Description = dto.Description;
            ticket.ClientName = dto.ClientName;
            ticket.Location = dto.Location;
            ticket.Category = dto.Category;
            ticket.RaisedBy = dto.RaisedBy;
            ticket.AssignedTo = dto.AssignedTo;
            ticket.Priority = dto.Priority;
            ticket.Status = dto.Status;
            ticket.Resolution = dto.Resolution;
            ticket.TimeRaised = dto.TimeRaised;
            ticket.AssignedToUserId = dto.AssignedToUserId;

            await _context.SaveChangesAsync();

            return new TicketDTO
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                Description = ticket.Description,
                ClientName = ticket.ClientName,
                Location = ticket.Location,
                Category = ticket.Category,
                RaisedBy = ticket.RaisedBy,
                AssignedTo = ticket.AssignedTo,
                Priority = ticket.Priority,
                Status = ticket.Status,
                Resolution = ticket.Resolution,
                DateRaised = ticket.DateRaised,
                TimeRaised = ticket.TimeRaised,
                CreatedByName = ticket.CreatedByUser != null ? ticket.CreatedByUser.Username : string.Empty,
                AssignedToName = ticket.AssignedToUser != null ? ticket.AssignedToUser.Username : string.Empty,
                CreatedByUserId = ticket.CreatedByUserId,
                AssignedToUserId = ticket.AssignedToUserId
            };
        }

        // -------------------------
        // Delete ticket
        // -------------------------
        public async Task<bool> DeleteAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return false;

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
