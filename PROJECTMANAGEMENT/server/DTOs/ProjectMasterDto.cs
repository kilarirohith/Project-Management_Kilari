namespace server.DTOs
{
    public class ProjectMasterDTO
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? Description { get; set; }
        
        // Needed for the Dropdown (to pre-select value)
        public int ClientId { get; set; }
        
        // Needed for the Table (to display text instead of ID)
        public string ClientName { get; set; } = null!;
    }

    public class CreateProjectMasterDTO
    {
        public string ProjectName { get; set; } = null!;
        public string? Description { get; set; }
        public int ClientId { get; set; } // The ID selected from the dropdown
    }
}