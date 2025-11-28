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
        public static string GenerateJwtToken(User user, IConfiguration config)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (config == null) throw new ArgumentNullException(nameof(config));

            var keyString = config["Jwt:Key"];
            var issuer = config["Jwt:Issuer"];
            var audience = config["Jwt:Audience"];
            var validityMinutesString = config["Jwt:TokenValidityMinutes"];

            if (string.IsNullOrEmpty(keyString) ||
                string.IsNullOrEmpty(issuer) ||
                string.IsNullOrEmpty(audience) ||
                string.IsNullOrEmpty(validityMinutesString))
            {
                throw new InvalidOperationException("JWT configuration is missing!");
            }

            if (!double.TryParse(validityMinutesString, out double validityMinutes))
                throw new InvalidOperationException("Invalid TokenValidityMinutes in config");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? user.FullName ?? "Unknown"),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
            };

            // Permission claims from RolePermissions
            if (user.Role?.RolePermissions != null)
            {
                foreach (var rp in user.Role.RolePermissions)
                {
                    if (rp.CanRead)
                        claims.Add(new Claim("Permission", $"{rp.Module}:Read"));
                    if (rp.CanCreate)
                        claims.Add(new Claim("Permission", $"{rp.Module}:Create"));
                    if (rp.CanUpdate)
                        claims.Add(new Claim("Permission", $"{rp.Module}:Update"));
                    if (rp.CanDelete)
                        claims.Add(new Claim("Permission", $"{rp.Module}:Delete"));
                }
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(validityMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        public static bool VerifyPassword(string password, string hashed) =>
            BCrypt.Net.BCrypt.Verify(password, hashed);
    }
}
