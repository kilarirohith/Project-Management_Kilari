namespace server.DTOs
{
    public class ProjectMasterDTO
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class CreateProjectMasterDTO
    {
        public string ProjectName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
