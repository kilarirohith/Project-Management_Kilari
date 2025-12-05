
using System;

namespace server.DTOs
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public string? ProjectType { get; set; }

        public int ClientId { get; set; }
        public string? ClientName { get; set; }

        public string? ClientLocation { get; set; }
        public string? Unit { get; set; }
        public string? Milestone { get; set; }

        public DateTime? PlanStartDate { get; set; }
        public DateTime? PlanEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
         public int? ElapsedDays { get; set; }

        public string Status { get; set; } = "Running";
    }

    public class CreateProjectDTO
    {
        public string ProjectName { get; set; } = string.Empty;
        public string? ProjectCode { get; set; }
        public string? ProjectType { get; set; }

        public int ClientId { get; set; }

        public string? ClientLocation { get; set; }
        public string? Unit { get; set; }
        public string? Milestone { get; set; }

        public DateTime? PlanStartDate { get; set; }
        public DateTime? PlanEndDate { get; set; }

        public string? Status { get; set; } = "Running";
    }
    
    public class UpdateProjectDTO : CreateProjectDTO
    {
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public int? ElapsedDays { get; set; }
   }
}
