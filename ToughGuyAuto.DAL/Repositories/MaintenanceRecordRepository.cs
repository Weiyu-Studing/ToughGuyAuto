using Microsoft.EntityFrameworkCore;
using ToughGuyAuto.DAL.Data;
using ToughGuyAuto.DAL.Interfaces;
using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Repositories;

public class MaintenanceRecordRepository : IMaintenanceRecordRepository
{
    private readonly ToughGuyAutoDbContext _context;

    public MaintenanceRecordRepository(ToughGuyAutoDbContext context)
    {
        _context = context;
    }
    public async Task<List<MaintenanceRecord>> GetAllAsync()
    {
        return await _context.MaintenanceRecords
            .Include(m => m.Vehicle)
            .Include(m => m.ServiceTypes)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<List<MaintenanceRecord>> GetByUserIdAsync(string userId)
    {
        return await _context.MaintenanceRecords
            .Include(m => m.Vehicle)
            .Include(m => m.ServiceTypes)
            .Where(m => m.Vehicle.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<MaintenanceRecord?> GetByIdAsync(int id)
    {
        return await _context.MaintenanceRecords
            .Include(m => m.Vehicle)
            .Include(m => m.ServiceTypes)
            .FirstOrDefaultAsync(
                m => m.MaintenanceRecordId == id);
    }

    public async Task AddAsync(MaintenanceRecord record)
    {
        _context.MaintenanceRecords.Add(record);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MaintenanceRecord record)
    {
        _context.MaintenanceRecords.Update(record);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var record = await _context.MaintenanceRecords
            .FirstOrDefaultAsync(
                m => m.MaintenanceRecordId == id);

        if (record != null)
        {
            _context.MaintenanceRecords.Remove(record);
            await _context.SaveChangesAsync();
        }
    }
}
