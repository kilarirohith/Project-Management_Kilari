using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class ProjectTask
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Foreign key for Project
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; } = null!;

        // Assigned user
        public int? AssignedToUserId { get; set; }

        [ForeignKey("AssignedToUserId")]
        public User? AssignedToUser { get; set; }

        public string Priority { get; set; } = "Normal"; // Low, Normal, High
        public string Status { get; set; } = "Open";     // Open, In Progress, Closed

        public DateTime? DueDate { get; set; }

        // When created (used as "Raised Date" in UI)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
