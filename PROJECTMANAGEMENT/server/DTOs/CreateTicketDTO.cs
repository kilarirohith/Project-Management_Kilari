public class CreateTicketDTO
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public string RaisedBy { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;

    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Open";
    public string Resolution { get; set; } = string.Empty;

    public string TimeRaised { get; set; } = string.Empty;

    public int CreatedByUserId { get; set; }      // ✅ now set from token
    public int? AssignedToUserId { get; set; }
}
