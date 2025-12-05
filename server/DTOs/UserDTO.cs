namespace server.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }

    public class RegisterUserDTO
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
    }

    public class LoginDTO
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class UpdateUserDTO
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
        public string? Password { get; set; } 
    }

    // NEW: simple user DTO for dropdowns
    public class SimpleUserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
         public string RoleName { get; set; } = string.Empty; 
    }
}
