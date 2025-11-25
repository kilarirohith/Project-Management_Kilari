namespace server.DTOs
{
    public class CreateApprovalDeskDTO
{
    public string Status { get; set; } = "Pending";
    public string Remarks { get; set; } = string.Empty;
    public int VendorWorkId { get; set; }
    public int ProjectId { get; set; }
}

}
