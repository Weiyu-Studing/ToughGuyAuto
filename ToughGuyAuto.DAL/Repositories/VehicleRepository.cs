using Microsoft.EntityFrameworkCore;
using ToughGuyAuto.DAL.Data;
using ToughGuyAuto.DAL.Interfaces;
using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly ToughGuyAutoDbContext _context;

    public VehicleRepository(ToughGuyAutoDbContext context)
    {
        _context = context;
    }

    // Return every vehicle.
    // AsNoTracking is used because the Index page only reads the data.
    public async Task<List<Vehicle>> GetAllAsync()
    {
        return await _context.Vehicles
            .AsNoTracking()
            .ToListAsync();
    }

    // Where filters the Vehicles table by UserId
    public async Task<List<Vehicle>> GetByUserIdAsync(string userId)
    {
        return await _context.Vehicles
            .Where(v => v.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    // FirstOrDefaultAsync returns one matching vehicle, or null if a vehicle with this ID does not exist.
    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.VehicleId == id);
    }
    public async Task AddAsync(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.VehicleId == id);

        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
    }
}
