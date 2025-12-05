
namespace server.DTOs
{
    public class VendorWorkDTO
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string ProjectName { get; set; } = string.Empty;  
        public string Category { get; set; } = null!;

        public string CoordinatorName { get; set; } = string.Empty; 

        public string Remarks { get; set; } = string.Empty;

        public int VendorId { get; set; }
        public string VendorName { get; set; } = null!;

        public string ApprovalStatus { get; set; } = "Pending";
        public string? ReportFilePath { get; set; }
    }

    public class CreateVendorWorkDTO
    {
        public DateTime Date { get; set; }
        public string ProjectName { get; set; } = string.Empty;   
        public string Category { get; set; } = null!;

        public string CoordinatorName { get; set; } = string.Empty; 
        public string Remarks { get; set; } = string.Empty;

        public int VendorId { get; set; }
    }

    public class VendorWorkFilterParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string? Status { get; set; }
        public int? VendorId { get; set; }
        public string? VendorName { get; set; }
    }
}
