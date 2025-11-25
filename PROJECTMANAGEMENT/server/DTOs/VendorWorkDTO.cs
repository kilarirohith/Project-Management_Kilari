namespace server.DTOs
{
    // Used for Reading (includes Vendor Name)
    public class VendorWorkDTO
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? WorkDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = null!;
        
        public int VendorId { get; set; }
        public string VendorName { get; set; } = null!;
    }

    // Used for Creating/Updating
    public class CreateVendorWorkDTO
    {
        public string ProjectName { get; set; } = null!;
        public string? WorkDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Pending";
        public int VendorId { get; set; }
    }
}
