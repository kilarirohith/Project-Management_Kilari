namespace server.DTOs
{
    public class VendorDTO
    {
        public int Id { get; set; }
        public string VendorName { get; set; } = null!;
        public string? VendorLocation { get; set; }
        public string? VendorGst { get; set; }
        public string Email { get; set; } = null!;
        public string? Spoc { get; set; }

        public int UserId { get; set; }   
    }

    public class CreateVendorDTO
    {
        public int UserId { get; set; }   

        public string VendorName { get; set; } = null!;
        public string? VendorLocation { get; set; }
        public string? VendorGst { get; set; }
        public string Email { get; set; } = null!;
        public string? Spoc { get; set; }
    }
}
