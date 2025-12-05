using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string Username { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public UserSettings? Settings { get; set; } 

        public ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();
        public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
    }
}
