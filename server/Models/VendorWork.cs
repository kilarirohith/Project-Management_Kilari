using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class VendorWork
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        
        [Required]
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = null!;

       
        [Required]
        public string CoordinatorName { get; set; } = string.Empty;

        public string Remarks { get; set; } = string.Empty;

        
        [Required]
        public int VendorId { get; set; }

        [ForeignKey(nameof(VendorId))]
        public Vendor Vendor { get; set; } = null!;

        [Required]
        public string ApprovalStatus { get; set; } = "Pending";

        public string? ReportFilePath { get; set; }
    }
}
