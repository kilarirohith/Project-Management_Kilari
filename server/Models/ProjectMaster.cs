using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class ProjectMaster
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProjectName { get; set; } = null!;

        public string? Description { get; set; }
    }
}
