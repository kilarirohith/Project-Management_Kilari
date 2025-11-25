using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        // Human-readable ticket code (e.g. TKT-001)
        [Required]
        [MaxLength(50)]
        public string TicketNumber { get; set; } = string.Empty;

        // Basic fields
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // Additional metadata
        [MaxLength(200)]
        public string ClientName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(100)]
        public string RaisedBy { get; set; } = string.Empty;

        [MaxLength(100)]
        public string AssignedTo { get; set; } = string.Empty;

        // Status / SLA
        [Required]
        [MaxLength(50)]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Open"; // Open, Closed, On Hold

        public string Resolution { get; set; } = string.Empty;

        // Time info
        public DateTime DateRaised { get; set; } = DateTime.Now;

        [MaxLength(20)]
        public string TimeRaised { get; set; } = string.Empty;

        // Relations
        public int CreatedByUserId { get; set; }
        public int? AssignedToUserId { get; set; }

        [ForeignKey("CreatedByUserId")]
        public User CreatedByUser { get; set; } = null!;

        [ForeignKey("AssignedToUserId")]
        public User? AssignedToUser { get; set; }
    }
}
