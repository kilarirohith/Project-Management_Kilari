using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using server.Authorization;
using server.DTOs;
using server.Services.Interfaces;
using server.Data;
using server.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VendorWorkController : ControllerBase
    {
        private readonly IVendorWorkService _vendorWorkService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public VendorWorkController(
            IVendorWorkService vendorWorkService,
            AppDbContext context,
            IWebHostEnvironment env)
        {
            _vendorWorkService = vendorWorkService;
            _context = context;
            _env = env;
        }

        // Small helpers to read claims safely
        private string? GetRoleFromClaims()
        {
            return User?.Claims?.FirstOrDefault(c =>
                       c.Type == ClaimTypes.Role ||
                       c.Type == "role")?.Value;
        }

        private int? GetUserIdFromClaims()
        {
            var claim = User?.Claims?.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "sub" ||
                c.Type == "userId");

            if (claim == null) return null;
            if (int.TryParse(claim.Value, out var id)) return id;
            return null;
        }

        // ---------- GET: api/VendorWork (paged + filtered) ----------
        [HttpGet]
        [PermissionAuthorize("VendorWork", "Read")]
        public async Task<IActionResult> GetAll([FromQuery] VendorWorkFilterParams filter)
        {
            var role = GetRoleFromClaims();
            var isVendor = string.Equals(role, "Vendor", StringComparison.OrdinalIgnoreCase);
            int? vendorId = null;

            if (isVendor)
            {
                var userId = GetUserIdFromClaims();
                if (userId == null)
                {
                    return Forbid("Missing user id in token.");
                }

                vendorId = await _context.Vendors
                    .Where(v => v.UserId == userId.Value)
                    .Select(v => (int?)v.Id)
                    .FirstOrDefaultAsync();

                if (!vendorId.HasValue || vendorId.Value == 0)
                {
                    return Ok(new PagedResult<VendorWorkDTO>
                    {
                        Items = new List<VendorWorkDTO>(),
                        TotalCount = 0
                    });
                }
            }

            var result = await _vendorWorkService.GetPagedAsync(filter, vendorId, isVendor);
            return Ok(result);
        }

        // ---------- GET: api/VendorWork/{id} ----------
        [HttpGet("{id}")]
        [PermissionAuthorize("VendorWork", "Read")]
        public async Task<IActionResult> GetById(int id)
        {
            var vendorWork = await _vendorWorkService.GetByIdAsync(id);
            if (vendorWork is null)
                return NotFound(new { message = "Vendor Work not found" });

            return Ok(vendorWork);
        }

        // ---------- POST: api/VendorWork ----------
        [HttpPost]
        [PermissionAuthorize("VendorWork", "Create")]
        public async Task<IActionResult> Create([FromBody] CreateVendorWorkDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _vendorWorkService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // ---------- PUT: api/VendorWork/{id} ----------
        [HttpPut("{id}")]
        [PermissionAuthorize("VendorWork", "Update")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateVendorWorkDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _vendorWorkService.UpdateAsync(id, dto);
            if (updated is null)
                return NotFound(new { message = "Vendor Work not found" });

            return Ok(updated);
        }

        // ---------- DELETE: api/VendorWork/{id} ----------
        [HttpDelete("{id}")]
        [PermissionAuthorize("VendorWork", "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _vendorWorkService.DeleteAsync(id);
            if (!success)
                return NotFound(new { message = "Vendor Work not found" });

            return NoContent();
        }

        // ========== REPORT UPLOAD ==========
        // POST: api/VendorWork/{id}/report
        [HttpPost("{id}/report")]
        [PermissionAuthorize("VendorWork", "Update")]
        public async Task<IActionResult> UploadReport(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var vw = await _context.VendorWorks
                .Include(x => x.Vendor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (vw == null)
                return NotFound("Vendor work not found.");

           
            if (!string.Equals(vw.ApprovalStatus, "Approved", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Report can be uploaded only after approval.");

            var role = GetRoleFromClaims();
            var isVendor = string.Equals(role, "Vendor", StringComparison.OrdinalIgnoreCase);

            if (isVendor)
            {
                var userId = GetUserIdFromClaims();
                if (userId == null)
                    return Forbid("Missing user id.");

                if (vw.Vendor.UserId != userId.Value)
                    return Forbid("You are not allowed to upload report for this work.");
            }

            
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            var root = _env.WebRootPath ?? _env.ContentRootPath;
            var folder = Path.Combine(root, "vendor-reports");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // if an old file exists, delete it before saving new one
            if (!string.IsNullOrEmpty(vw.ReportFilePath))
            {
                var oldFull = Path.Combine(root, vw.ReportFilePath);
                if (System.IO.File.Exists(oldFull))
                {
                    System.IO.File.Delete(oldFull);
                }
            }

            var fileName = $"vw_{id}_{DateTime.UtcNow:yyyyMMddHHmmss}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            vw.ReportFilePath = Path.Combine("vendor-reports", fileName)
                                    .Replace("\\", "/");
            await _context.SaveChangesAsync();

            return Ok(new { message = "Report uploaded", path = vw.ReportFilePath });
        }

        // ========== REPORT DOWNLOAD ==========
        // GET: api/VendorWork/{id}/report
        [HttpGet("{id}/report")]
        [PermissionAuthorize("VendorWork", "Read")]
        public async Task<IActionResult> DownloadReport(int id)
        {
            var vw = await _context.VendorWorks
                .Include(x => x.Vendor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (vw == null)
                return NotFound("Vendor work not found.");

            if (string.IsNullOrEmpty(vw.ReportFilePath))
                return NotFound("No report uploaded for this vendor work.");

            var role = GetRoleFromClaims();
            var isVendor = string.Equals(role, "Vendor", StringComparison.OrdinalIgnoreCase);

            if (isVendor)
            {
                var userId = GetUserIdFromClaims();
                if (userId == null)
                    return Forbid("Missing user id.");

                if (vw.Vendor.UserId != userId.Value)
                    return Forbid("You are not allowed to download this report.");
            }

            var root = _env.WebRootPath ?? _env.ContentRootPath;
            var fullPath = Path.Combine(root, vw.ReportFilePath);
            if (!System.IO.File.Exists(fullPath))
                return NotFound("Report file not found on server.");

            var ext = Path.GetExtension(fullPath).ToLowerInvariant();

            
            var contentType = ext switch
            {
                ".pdf"  => "application/pdf",
                ".jpg"  => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png"  => "image/png",
                ".doc"  => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls"  => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt"  => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".txt"  => "text/plain",
                ".csv"  => "text/csv",
                _       => "application/octet-stream"
            };

            var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            var downloadName = Path.GetFileName(fullPath);

            return File(bytes, contentType, downloadName);
        }
    }
}
