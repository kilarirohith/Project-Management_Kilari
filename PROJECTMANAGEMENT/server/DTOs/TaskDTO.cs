using System;

namespace server.DTOs
{
    public class TaskDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public string Status { get; set; } = null!;
        public string Priority { get; set; } = null!;

        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;

        public int? AssignedToUserId { get; set; }
        public string? AssignedUserName { get; set; }

        public DateTime CreatedAt { get; set; }        // Raised Date
        public DateTime? DueDate { get; set; }         // Expected Closure

        public int? Progress { get; set; }             // from TaskTracker
    }
}
