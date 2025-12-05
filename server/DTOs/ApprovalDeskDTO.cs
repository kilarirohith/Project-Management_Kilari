// server/DTOs/ApprovalDeskDTO.cs
namespace server.DTOs
{
    public class ApprovalDeskDTO
    {
        public int Id { get; set; }
        public string Status { get; set; } = "Pending";
        public string Remarks { get; set; } = string.Empty;

        public int VendorWorkId { get; set; }

        public DateTime Date { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public string CoordinatorName { get; set; } = string.Empty; // from VendorWork.CoordinatorName

        public string? ReportFilePath { get; set; }
    }

    public class CreateApprovalDeskDTO
    {
        public string Status { get; set; } = "Pending";
        public string Remarks { get; set; } = string.Empty;
        public int VendorWorkId { get; set; }
    }
}
