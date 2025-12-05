// server/Models/Project.cs
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

        public string ProjectCode { get; set; } = string.Empty;

        public string? ProjectType { get; set; }

      
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; } = null!;

        public string? ClientLocation { get; set; }
        public string? Unit { get; set; }

        public string? Milestone { get; set; }

        public DateTime? PlanStartDate { get; set; }
        public DateTime? PlanEndDate { get; set; }

        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }

        public int? ElapsedDays { get; set; }

        
        public string Status { get; set; } = "Running";

        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}
