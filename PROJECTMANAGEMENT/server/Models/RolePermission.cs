namespace server.Models
{
    public class RolePermission
    {
        public int Id { get; set; }
        
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!; // Foreign Key Link

        public string Module { get; set; } = null!; // e.g. "Dashboard"
        
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}