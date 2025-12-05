using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using server.DTOs;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _db;

        public TicketService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<TicketDTO>> GetAllAsync()
        {
            var tickets = await _db.Tickets
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            return tickets.Select(ToDto);
        }

        public async Task<TicketDTO?> GetByIdAsync(int id)
        {
            var ticket = await _db.Tickets.FindAsync(id);
            return ticket == null ? null : ToDto(ticket);
        }

        public async Task<TicketDTO> CreateAsync(CreateTicketDTO dto, int currentUserId)
        {
            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                ClientName = dto.ClientName,
                Location = dto.Location,
                Category = dto.Category,
                RaisedBy = dto.RaisedBy,
                AssignedTo = dto.AssignedTo,
                Priority = dto.Priority,
                Status = dto.Status,
                Resolution = dto.Resolution,
                DateRaised = dto.DateRaised ?? DateTime.UtcNow,
                TimeRaised = dto.TimeRaised,
                DateClosed = dto.DateClosed,
                TimeClosed = dto.TimeClosed,
                CreatedByUserId = currentUserId,
                AssignedToUserId = dto.AssignedToUserId
            };

           
            ticket.TicketNumber = DateTime.UtcNow.ToString("ddMMyyyyHHmm");

            ComputeDurations(ticket);

            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();

            return ToDto(ticket);
        }

        public async Task<TicketDTO?> UpdateAsync(int id, CreateTicketDTO dto, int currentUserId)
        {
            var ticket = await _db.Tickets.FindAsync(id);
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

            ticket.DateRaised = dto.DateRaised ?? ticket.DateRaised;
            if (!string.IsNullOrEmpty(dto.TimeRaised))
            {
                ticket.TimeRaised = dto.TimeRaised;
            }

            ticket.DateClosed = dto.DateClosed;
            ticket.TimeClosed = dto.TimeClosed;
            ticket.AssignedToUserId = dto.AssignedToUserId;

            ComputeDurations(ticket);

            await _db.SaveChangesAsync();

            return ToDto(ticket);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ticket = await _db.Tickets.FindAsync(id);
            if (ticket == null) return false;

            _db.Tickets.Remove(ticket);
            await _db.SaveChangesAsync();
            return true;
        }

      private static void ComputeDurations(Ticket t)
{
   
    if (t.DateClosed == default(DateTime) ||
        string.IsNullOrWhiteSpace(t.TimeClosed) ||
        t.DateRaised == default(DateTime) ||
        string.IsNullOrWhiteSpace(t.TimeRaised))
    {
        t.TotalHoursElapsed = null;
        t.TotalDaysElapsed = null;
        return;
    }

    try
    {
        var start = DateTime.Parse($"{t.DateRaised:yyyy-MM-dd} {t.TimeRaised}");
        var end   = DateTime.Parse($"{t.DateClosed.Value:yyyy-MM-dd} {t.TimeClosed}");

        var diff = end - start;

       
        var totalHours = Math.Abs(diff.TotalHours);
        var totalDays  = Math.Abs(diff.TotalDays);

        t.TotalHoursElapsed = Math.Round(totalHours, 2);
        t.TotalDaysElapsed  = (int)Math.Floor(totalDays);
    }
    catch
    {
        t.TotalHoursElapsed = null;
        t.TotalDaysElapsed = null;
    }
}


        private static TicketDTO ToDto(Ticket t)
        {
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
                DateClosed = t.DateClosed,
                TimeClosed = t.TimeClosed,
                TotalHoursElapsed = t.TotalHoursElapsed,
                TotalDaysElapsed = t.TotalDaysElapsed,
                CreatedByUserId = t.CreatedByUserId,
                AssignedToUserId = t.AssignedToUserId
            };
        }
    }
}
