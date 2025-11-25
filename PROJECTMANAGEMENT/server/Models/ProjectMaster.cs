using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class ProjectMaster
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProjectName { get; set; } = null!;

        public string? Description { get; set; }

        // --- Relationship: Link to Client Master ---
        [Required]
        public int ClientId { get; set; }
        
        [ForeignKey("ClientId")]
        public Client Client { get; set; } = null!;
    }
}