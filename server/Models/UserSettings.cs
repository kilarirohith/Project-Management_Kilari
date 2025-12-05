using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class UserSettings
    {
        [Key]
        public int Id { get; set; }

        
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public bool DarkMode { get; set; } = false;
        public bool EmailNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = false;
        public bool TwoFactorEnabled { get; set; } = false;

        public string DefaultProjectView { get; set; } = "kanban"; // "kanban" | "list" | "calendar"

        public int SessionTimeoutMinutes { get; set; } = 30;

    
    }
}
