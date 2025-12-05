using System;

namespace server.DTOs
{
    public class CreateTaskDTO
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string? Priority { get; set; } = "Normal";
        public string? Status { get; set; } = "Open";

        public int ProjectId { get; set; }
        public int? AssignedToUserId { get; set; }

        public DateTime? DueDate { get; set; }          
        public DateTime? ActualClosureDate { get; set; }

       
        public string? Type { get; set; }               
        public string? ProduceStep { get; set; }
        public string? SampleData { get; set; }
        public string? AcceptanceCriteria { get; set; }
        public string? TestingStatus { get; set; }
        public string? TestingDoneBy { get; set; }
    }
}
