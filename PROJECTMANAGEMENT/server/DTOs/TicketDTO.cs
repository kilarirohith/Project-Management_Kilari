using System;

namespace server.DTOs
{
    public class TicketDTO
    {
        public int Id { get; set; }

        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string ClientName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public string RaisedBy { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;

        public DateTime DateRaised { get; set; }
        public string TimeRaised { get; set; } = string.Empty;

        public string CreatedByName { get; set; } = string.Empty;
        public string AssignedToName { get; set; } = string.Empty;

        public int CreatedByUserId { get; set; }
        public int? AssignedToUserId { get; set; }
    }
}
