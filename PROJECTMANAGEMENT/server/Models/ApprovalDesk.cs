using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class ApprovalDesk
    {
        [Key]
        public int Id { get; set; }

        public string Status { get; set; } = "Pending"; // Default value

        public string Remarks { get; set; } = string.Empty;

        public int VendorWorkId { get; set; }
        [ForeignKey("VendorWorkId")]
        public VendorWork VendorWork { get; set; } = null!;

        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; } = null!;
    }
}
