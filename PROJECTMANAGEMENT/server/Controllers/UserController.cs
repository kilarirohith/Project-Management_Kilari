using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // 1. LOGIN (Public)
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
                    role = result.User.Role?.Name ?? "User"
                }
            });
        }

        // 2. CREATE (Admin Only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = await _userService.RegisterUserAsync(dto);
                return Ok(new { message = "User created successfully", userId = user.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 3. GET ALL (Admin Only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();

            var response = users.Select(u => new 
            {
                id = u.Id,
                fullName = u.FullName,
                email = u.Email,
                role = u.Role == null ? null : new { id = u.Role.Id, name = u.Role.Name }
            });

            return Ok(response);
        }

        // 4. GET BY ID (Admin Only)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            return Ok(new 
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                // ✅ Corrected: uses 'user' variable, not 'u'
                role = user.Role == null ? null : new { id = user.Role.Id, name = user.Role.Name }
            });
        }

        // 5. UPDATE (Admin Only) - ✅ Added
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDTO dto)
        {
            try 
            {
                var updatedUser = await _userService.UpdateUserAsync(id, dto);
                if (updatedUser == null) return NotFound(new { message = "User not found" });
                
                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 6. DELETE (Admin Only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return NotFound(new { message = "User not found" });

            return Ok(new { message = "User deleted successfully" });
        }
    }
}