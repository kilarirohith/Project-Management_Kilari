using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace server.Models
{
    public class VendorWork
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProjectName { get; set; } = null!;

        public string? WorkDescription { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string Status { get; set; } = "Pending";

        [Required]
        public int VendorId { get; set; }

        [ForeignKey("VendorId")]
        [JsonIgnore]
        public Vendor Vendor { get; set; } = null!;
    }
}
