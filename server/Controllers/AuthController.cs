using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using server.Data;
using server.Models;
using server.DTOs;  
using server.Utils; 

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // -------- REGISTER ----------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (registerDto == null)
                return BadRequest("Invalid user data.");

            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return BadRequest("Email already exists.");

            // Find Role (by name)
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == registerDto.Role)
                       ?? await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

            if (role == null)
                return BadRequest("Default role 'User' not found.");

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Username = registerDto.Email, 
                PasswordHash = AuthHelper.HashPassword(registerDto.Password),
                RoleId = role.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

           

            return Ok(new { message = "User registered successfully" });
        }

        // -------- LOGIN ----------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
                return BadRequest("Invalid login data.");

            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Settings)    
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !AuthHelper.VerifyPassword(loginDto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            // Safe read of SessionTimeoutMinutes
            int sessionTimeout = (user.Settings?.SessionTimeoutMinutes ?? 30) > 0
                ? user.Settings!.SessionTimeoutMinutes
                : 30;

            // Generate token with per-user timeout
            var token = AuthHelper.GenerateJwtToken(user, sessionTimeout, _config);

            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    role = user.Role?.Name ?? "User"
                }
            });
        }
    }
}
