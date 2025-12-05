namespace server.DTOs
{
    public class RolePermissionDTO
    {
        public string Module { get; set; } = null!;
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class RoleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<RolePermissionDTO> Permissions { get; set; } = new();
    }
}