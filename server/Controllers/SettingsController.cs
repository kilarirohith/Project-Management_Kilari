using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Services.Interfaces;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        private int GetUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idStr!);
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserSettingsDto>> GetMySettings()
        {
            var userId = GetUserId();
            var dto = await _settingsService.GetForUserAsync(userId);
            return Ok(dto);
        }

        [HttpPut("me")]
        public async Task<ActionResult<UserSettingsDto>> UpdateMySettings([FromBody] UserSettingsDto dto)
        {
            var userId = GetUserId();
            var updated = await _settingsService.UpsertForUserAsync(userId, dto);
            return Ok(updated);
        }
    }
}
