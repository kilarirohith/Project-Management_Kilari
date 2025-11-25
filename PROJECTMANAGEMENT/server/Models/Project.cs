using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        public string ProjectName { get; set; } = string.Empty;

        // Code shown in table (e.g. POL-PRO-017 or auto PRJ-001)
        public string ProjectCode { get; set; } = string.Empty;

        // e.g. ProEfficient, Digitization, Automation
        public string? ProjectType { get; set; }

        // FK → Client
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; } = null!;

        // From Client's Location / Unit
        public string? ClientLocation { get; set; }
        public string? Unit { get; set; }

        // Milestone name from MilestoneMaster
        public string? Milestone { get; set; }

        // Plan dates
        public DateTime? PlanStartDate { get; set; }
        public DateTime? PlanEndDate { get; set; }

        // Actual dates
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }

        /// <summary>
        /// Running, Completed, Delayed, OnHold, Pending
        /// </summary>
        public string Status { get; set; } = "Running";

        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}
