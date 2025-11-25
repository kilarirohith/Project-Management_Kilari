using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class MilestoneMaster
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Range(0, 100)]
        public int Weightage { get; set; }
    }
}