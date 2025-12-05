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

        public DateTime CreatedAt { get; set; }          
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
