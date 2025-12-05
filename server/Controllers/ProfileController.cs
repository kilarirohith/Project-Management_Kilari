
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Services.Interfaces;
using System.Security.Claims;

namespace server.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class ProfileController : ControllerBase {
    private readonly IProfileService _profileService;
    private readonly IWebHostEnvironment _env;

    public ProfileController(IProfileService profileService,
                             IWebHostEnvironment env) {
      _profileService = profileService;
      _env = env;
    }

    private int GetUserId() {
      var idClaim =
          User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
      if (idClaim == null)
        throw new Exception("User id not found in token");
      return int.Parse(idClaim.Value);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMe() {
      var userId = GetUserId();
      var profile = await _profileService.GetProfileAsync(userId);
      if (profile == null)
        return NotFound();
      return Ok(profile);
    }

    // server/Controllers/ProfileController.cs

    [HttpPut("me")]
    [RequestSizeLimit(5_000_000)]  // 5 MB limit
    public async Task<ActionResult<UserProfileDto>> UpdateMe(
        [FromForm] string fullName, [FromForm] string email,
        [FromForm] IFormFile? avatar,
        [FromForm] bool removeAvatar = false  // 🔴 NEW: supports delete photo
    ) {
      var userId = GetUserId();

      // 1️⃣ Get current profile (to know old avatar path)
      var currentProfile = await _profileService.GetProfileAsync(userId);
      var oldAvatarUrl =
          currentProfile?.AvatarUrl;  // e.g. "/uploads/avatars/abc.jpg"

      string? newAvatarUrl = null;
      var webRoot = _env.WebRootPath ?? "wwwroot";

      // 2️⃣ If user clicked "Remove Photo" (no new file)
      if (removeAvatar && !string.IsNullOrWhiteSpace(oldAvatarUrl) &&
          (avatar == null || avatar.Length == 0)) {
        var oldPhysicalPath =
            Path.Combine(webRoot, oldAvatarUrl.TrimStart('/'));
        if (System.IO.File.Exists(oldPhysicalPath)) {
          System.IO.File.Delete(oldPhysicalPath);
        }

        // keep newAvatarUrl = null → DB avatar will be cleared
      }

      // 3️⃣ If a new file is uploaded -> delete old file + save new one
      if (avatar != null && avatar.Length > 0) {
        // 3a) Delete old file if present
        if (!string.IsNullOrWhiteSpace(oldAvatarUrl)) {
          var oldPhysicalPath =
              Path.Combine(webRoot, oldAvatarUrl.TrimStart('/'));
          if (System.IO.File.Exists(oldPhysicalPath)) {
            System.IO.File.Delete(oldPhysicalPath);
          }
        }

        // 3b) Save new file
        var uploadsRoot = Path.Combine(webRoot, "uploads", "avatars");
        Directory.CreateDirectory(uploadsRoot);

        var ext = Path.GetExtension(avatar.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var savePath = Path.Combine(uploadsRoot, fileName);

        await using (var stream = System.IO.File.Create(savePath)) {
          await avatar.CopyToAsync(stream);
        }

        // this is the URL stored in DB (and used by Angular)
        newAvatarUrl = $"/uploads/avatars/{fileName}";

        // because we now have a new avatar, don't treat as "remove"
        removeAvatar = false;
      }

      // 4️⃣ Update DB
      var updated = await _profileService.UpdateProfileAsync(
          userId, fullName, email, newAvatarUrl,
          removeAvatar  // 🔴 tells service to clear AvatarUrl if true & no new
                        // image
      );

      return Ok(updated);
    }
  }
}
