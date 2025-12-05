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

        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; } = null!;

        public int? AssignedToUserId { get; set; }
        [ForeignKey("AssignedToUserId")]
        public User? AssignedToUser { get; set; }

        public string Priority { get; set; } = "Normal";
        public string Status { get; set; } = "Open";

        public DateTime? DueDate { get; set; }              
        public DateTime? ActualClosureDate { get; set; }    

        
        public string? Type { get; set; }                   
        public string? ProduceStep { get; set; }
        public string? SampleData { get; set; }
        public string? AcceptanceCriteria { get; set; }
        public string? TestingStatus { get; set; }
        public string? TestingDoneBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
