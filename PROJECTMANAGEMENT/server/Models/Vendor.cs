using System.ComponentModel.DataAnnotations;

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

        [EmailAddress]
        public string Email { get; set; } = null!;
        
        public string? Spoc { get; set; }

        public ICollection<VendorWork> VendorWorks { get; set; } = new List<VendorWork>();
    }
}
