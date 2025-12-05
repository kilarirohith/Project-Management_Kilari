using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using server.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace server.Utils
{
    public static class AuthHelper
    {
        public static string GenerateJwtToken(User user, int timeoutMinutes, IConfiguration config)
        {
            var keyString = config["Jwt:Key"]
                            ?? throw new Exception("JWT:Key not configured");
            var issuer = config["Jwt:Issuer"];
            var audience = config["Jwt:Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? user.FullName ?? "Unknown"),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
            };

            if (user.Role?.RolePermissions != null)
            {
                foreach (var rp in user.Role.RolePermissions)
                {
                    if (rp.CanRead)   claims.Add(new Claim("Permission", $"{rp.Module}:Read"));
                    if (rp.CanCreate) claims.Add(new Claim("Permission", $"{rp.Module}:Create"));
                    if (rp.CanUpdate) claims.Add(new Claim("Permission", $"{rp.Module}:Update"));
                    if (rp.CanDelete) claims.Add(new Claim("Permission", $"{rp.Module}:Delete"));
                }
            }

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(timeoutMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public static string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        public static bool VerifyPassword(string password, string hashed) =>
            BCrypt.Net.BCrypt.Verify(password, hashed);
    }
}
