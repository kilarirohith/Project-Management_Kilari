

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace server.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public string ClientName { get; set; } = null!;
        public string? Gst { get; set; }
        public string Email { get; set; } = null!;

       public ICollection<Project> Projects { get; set; } = new List<Project>();
        // Navigation
        public List<ClientLocation> Locations { get; set; } = new();
    }

    public class ClientLocation
    {
        [Key]
        public int Id { get; set; }
        
        public string LocationName { get; set; } = null!;
        public string? Spoc { get; set; }

        // FK
        public int ClientId { get; set; }
        [JsonIgnore] // Prevent cycle
        public Client Client { get; set; } = null!;

        // Navigation
        public List<ClientUnit> Units { get; set; } = new();
    }

    public class ClientUnit
    {
        [Key]
        public int Id { get; set; }
        
        public string UnitName { get; set; } = null!;

        // FK
        public int ClientLocationId { get; set; }
        [JsonIgnore]
        public ClientLocation ClientLocation { get; set; } = null!;
    }
}