using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class Vendor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string VendorName { get; set; } = null!;

        public string? VendorLocation { get; set; }
        public string? VendorGst { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        public string? Spoc { get; set; }

        
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public ICollection<VendorWork> VendorWorks { get; set; } = new List<VendorWork>();
    }
}
