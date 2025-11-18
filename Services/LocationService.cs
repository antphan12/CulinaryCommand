using CulinaryCommand.Data;
using CulinaryCommand.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CulinaryCommand.Services
{
    public interface ILocationService
    {
        Task<List<Location>> GetAllAsync();
        Task<Location?> GetByIdAsync(int id);
        Task<Location> CreateAsync(Location location);
        Task UpdateAsync(Location location);
        Task DeleteAsync(int id);
    }

    public class LocationService : ILocationService
    {
        private readonly AppDbContext _context;

        public LocationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Location>> GetAllAsync()
        {
            return await _context.Locations
                .Include(l => l.Company)
                .ToListAsync();
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            return await _context.Locations
                .Include(l => l.Company)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Location> CreateAsync(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task UpdateAsync(Location location)
        {
            var existing = await _context.Locations.FindAsync(location.Id);
            if (existing == null) return;

            existing.Name = location.Name;
            existing.Address = location.Address;
            existing.City = location.City;
            existing.State = location.State;
            existing.ZipCode = location.ZipCode;
            existing.MarginEdgeKey = location.MarginEdgeKey;
            existing.CompanyId = location.CompanyId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var loc = await _context.Locations.FindAsync(id);
            if (loc == null) return;

            _context.Locations.Remove(loc);
            await _context.SaveChangesAsync();
        }
    }
}