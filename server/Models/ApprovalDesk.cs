using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class ApprovalDesk
    {
        [Key]
        public int Id { get; set; }

        public string Status { get; set; } = "Pending";
        public string Remarks { get; set; } = string.Empty;

        [Required]
        public int VendorWorkId { get; set; }

        [ForeignKey(nameof(VendorWorkId))]
        public VendorWork VendorWork { get; set; } = null!;

   
    }
}
