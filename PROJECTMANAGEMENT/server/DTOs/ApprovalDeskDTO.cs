namespace server.DTOs
{
    public class ApprovalDeskDTO
    {
        public int Id { get; set; }
        public string Status { get; set; } = null!;
        public string Remarks { get; set; } = null!;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;
        public int VendorWorkId { get; set; }
    }
}
