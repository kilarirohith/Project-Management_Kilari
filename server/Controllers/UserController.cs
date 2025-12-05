using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Authorization;
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // ---------- LOGIN ----------
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _userService.LoginUserAsync(dto);

            if (result.User == null || result.Token == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new
            {
                token = result.Token,
                user = new
                {
                    id = result.User.Id,
                    fullName = result.User.FullName,
                    email = result.User.Email,
                    role = result.User.Role?.Name ?? "User",
                    avatarUrl = result.User.AvatarUrl
                }
            });
        }

        // ---------- CREATE USER ----------
        [HttpPost]
        [Authorize]
        [PermissionAuthorize("Masters", "Create")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = await _userService.RegisterUserAsync(dto);
                return Ok(new { message = "User created successfully", userId = user.Id });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ---------- FORGOT PASSWORD ----------
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            await _userService.RequestPasswordResetAsync(dto.Email);
            return Ok(new { message = "If email exists, reset link sent" });
        }

        // ---------- RESET PASSWORD ----------
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var ok = await _userService.ResetPasswordAsync(dto.Token, dto.NewPassword);

            if (!ok) return BadRequest(new { message = "Invalid or expired token" });

            return Ok(new { message = "Password updated successfully" });
        }

        // ---------- GET ALL USERS ----------
        [HttpGet]
        [Authorize]
        [PermissionAuthorize("Masters", "Read")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();

            var response = users.Select(u => new
            {
                id = u.Id,
                fullName = u.FullName,
                email = u.Email,
                role = u.Role == null ? null : new { id = u.Role.Id, name = u.Role.Name },
                avatarUrl = u.AvatarUrl
            });

            return Ok(response);
        }

        // ---------- SIMPLE USERS (for dropdowns) ----------
        [HttpGet("simple")]
        [Authorize] 
        public async Task<IActionResult> GetSimpleUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            var result = users.Select(u => new SimpleUserDTO
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                 RoleName = u.Role != null ? u.Role.Name : string.Empty
            }) .ToList();

            return Ok(result);
        }

        // ---------- GET BY ID ----------
        [HttpGet("{id}")]
        [Authorize]
        [PermissionAuthorize("Masters", "Read")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            return Ok(new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                role = user.Role == null ? null : new { id = user.Role.Id, name = user.Role.Name },
                avatarUrl = user.AvatarUrl
            });
        }

        // ---------- UPDATE ----------
        [HttpPut("{id}")]
        [Authorize]
        [PermissionAuthorize("Masters", "Update")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDTO dto)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, dto);
                if (updatedUser == null) return NotFound(new { message = "User not found" });

                return Ok(new { message = "User updated successfully" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ---------- DELETE ----------
        [HttpDelete("{id}")]
        [Authorize]
        [PermissionAuthorize("Masters", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound(new { message = "User not found" });

            return Ok(new { message = "User deleted successfully" });
        }
    }
}
