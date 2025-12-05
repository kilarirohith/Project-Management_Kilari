using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoleDTO>> GetAllAsync()
        {
            var roles = await _context.Roles
                .Include(r => r.RolePermissions)
                .ToListAsync();

            return roles.Select(MapToDTO);
        }

        public async Task<RoleDTO?> GetByIdAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);
            return role == null ? null : MapToDTO(role);
        }

        public async Task<RoleDTO> CreateAsync(RoleDTO roleDTO)
        {
            // 1. Create Role
            var role = new Role
            {
                Name = roleDTO.Name,
                Description = roleDTO.Description
            };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync(); 

            // 2. Save Permissions
            if (roleDTO.Permissions != null && roleDTO.Permissions.Any())
            {
                var perms = roleDTO.Permissions.Select(p => new RolePermission
                {
                    RoleId = role.Id,
                    Module = p.Module,
                    CanCreate = p.CanCreate,
                    CanRead = p.CanRead,
                    CanUpdate = p.CanUpdate,
                    CanDelete = p.CanDelete
                });
                _context.RolePermissions.AddRange(perms);
                await _context.SaveChangesAsync();
            }

            return await GetByIdAsync(role.Id)!;
        }

        public async Task<RoleDTO?> UpdateAsync(int id, RoleDTO roleDTO)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) return null;

           
            role.Name = roleDTO.Name;
            role.Description = roleDTO.Description;

            
            _context.RolePermissions.RemoveRange(role.RolePermissions);
            
            if (roleDTO.Permissions != null)
            {
                var newPerms = roleDTO.Permissions.Select(p => new RolePermission
                {
                    RoleId = role.Id,
                    Module = p.Module,
                    CanCreate = p.CanCreate,
                    CanRead = p.CanRead,
                    CanUpdate = p.CanUpdate,
                    CanDelete = p.CanDelete
                });
                _context.RolePermissions.AddRange(newPerms);
            }

            await _context.SaveChangesAsync();
            return MapToDTO(role);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return false;

            
            var perms = _context.RolePermissions.Where(p => p.RoleId == id);
            _context.RolePermissions.RemoveRange(perms);

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        private static RoleDTO MapToDTO(Role role)
        {
            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = role.RolePermissions.Select(p => new RolePermissionDTO
                {
                    Module = p.Module,
                    CanCreate = p.CanCreate,
                    CanRead = p.CanRead,
                    CanUpdate = p.CanUpdate,
                    CanDelete = p.CanDelete
                }).ToList()
            };
        }
    }
}