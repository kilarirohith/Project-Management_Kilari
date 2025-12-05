namespace server.DTOs
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = null!;
        public string? Gst { get; set; }
        public string Email { get; set; } = null!;
        public List<LocationDTO> Locations { get; set; } = new();
        
    }

    public class LocationDTO
    {
        public int Id { get; set; }         
        public string LocationName { get; set; } = null!;
        public string? Spoc { get; set; }
        public List<UnitDTO> Units { get; set; } = new();
    }

    public class UnitDTO
    {
        public int Id { get; set; }         
        public string UnitName { get; set; } = null!;
    }
}
