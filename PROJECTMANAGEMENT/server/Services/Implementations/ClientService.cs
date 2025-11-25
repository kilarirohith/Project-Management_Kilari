using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;

        public ClientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClientDTO>> GetAllAsync()
        {
            var clients = await _context.Clients
                .Include(c => c.Locations)
                .ThenInclude(l => l.Units)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return clients.Select(MapToDTO).ToList();
        }

        public async Task<ClientDTO?> GetByIdAsync(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Locations)
                .ThenInclude(l => l.Units)
                .FirstOrDefaultAsync(c => c.Id == id);

            return client == null ? null : MapToDTO(client);
        }

        public async Task<ClientDTO> CreateAsync(ClientDTO dto)
        {
            var client = new Client
            {
                ClientName = dto.ClientName,
                Gst = dto.Gst,
                Email = dto.Email,
                Locations = dto.Locations.Select(l => new ClientLocation
                {
                    LocationName = l.LocationName,
                    Spoc = l.Spoc,
                    Units = l.Units.Select(u => new ClientUnit
                    {
                        UnitName = u.UnitName
                    }).ToList()
                }).ToList()
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return MapToDTO(client);
        }

        public async Task<ClientDTO?> UpdateAsync(int id, ClientDTO dto)
        {
            var client = await _context.Clients
                .Include(c => c.Locations)
                .ThenInclude(l => l.Units)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null) return null;

            // 1. Update Basic Info
            client.ClientName = dto.ClientName;
            client.Gst = dto.Gst;
            client.Email = dto.Email;

            // 2. Handle Nested Data (Strategy: Clear old, Add new)
            // Note: For high-performance apps, you would diff/merge, but this is safer for simple logic
            _context.ClientUnits.RemoveRange(client.Locations.SelectMany(l => l.Units));
            _context.ClientLocations.RemoveRange(client.Locations);

            client.Locations = dto.Locations.Select(l => new ClientLocation
            {
                LocationName = l.LocationName,
                Spoc = l.Spoc,
                Units = l.Units.Select(u => new ClientUnit
                {
                    UnitName = u.UnitName
                }).ToList()
            }).ToList();

            await _context.SaveChangesAsync();
            return MapToDTO(client);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return false;

            // Cascading delete should handle children if DB is configured, 
            // but explicit removal ensures safety in code.
            var locations = _context.ClientLocations.Where(l => l.ClientId == id).ToList();
            var units = _context.ClientUnits.Where(u => locations.Select(l => l.Id).Contains(u.ClientLocationId)).ToList();

            _context.ClientUnits.RemoveRange(units);
            _context.ClientLocations.RemoveRange(locations);
            _context.Clients.Remove(client);

            await _context.SaveChangesAsync();
            return true;
        }

        private static ClientDTO MapToDTO(Client c)
        {
            return new ClientDTO
            {
                Id = c.Id,
                ClientName = c.ClientName,
                Gst = c.Gst,
                Email = c.Email,
                Locations = c.Locations.Select(l => new LocationDTO
                {
                    Id = l.Id,
                    LocationName = l.LocationName,
                    Spoc = l.Spoc,
                    Units = l.Units.Select(u => new UnitDTO
                    {
                        Id = u.Id,
                        UnitName = u.UnitName
                    }).ToList()
                }).ToList()
            };
        }
    }
}