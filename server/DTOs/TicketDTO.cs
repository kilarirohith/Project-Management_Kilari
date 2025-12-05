using System;

namespace server.DTOs
{
    public class TicketDTO
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string? ClientName { get; set; }
        public string? Location { get; set; }
        public string? Category { get; set; }

        public string? RaisedBy { get; set; }
        public string? AssignedTo { get; set; }

        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Open";
        public string? Resolution { get; set; }

        public DateTime DateRaised { get; set; }
        public string? TimeRaised { get; set; }

        public DateTime? DateClosed { get; set; }
        public string? TimeClosed { get; set; }

        public double? TotalHoursElapsed { get; set; }
        public int? TotalDaysElapsed { get; set; }

        public int CreatedByUserId { get; set; }
        public int? AssignedToUserId { get; set; }
    }

    public class CreateTicketDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string? ClientName { get; set; }
        public string? Location { get; set; }
        public string? Category { get; set; }

        public string? RaisedBy { get; set; }
        public string? AssignedTo { get; set; }

        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Open";
        public string? Resolution { get; set; }

        public DateTime? DateRaised { get; set; }
        public string? TimeRaised { get; set; }

        public DateTime? DateClosed { get; set; }
        public string? TimeClosed { get; set; }

        public int? AssignedToUserId { get; set; }
    }

    public class UpdateTicketDTO : CreateTicketDTO
    {
        public string TicketNumber { get; set; } = string.Empty;
    }
}
