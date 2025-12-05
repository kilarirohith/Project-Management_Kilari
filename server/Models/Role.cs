namespace server.Models
{
    public class Role
    {
        public int Id { get; set; }              
        public string Name { get; set; } = null!; 
        public string? Description { get; set; }  

        
        public List<RolePermission> RolePermissions { get; set; } = new();
    }
}
