using Microsoft.EntityFrameworkCore;
using ToughGuyAuto.DAL.Data;
using ToughGuyAuto.DAL.Interfaces;
using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Repositories;

// This class is part of the data access layer. 
// It contains EF Core code for reading and modifying MaintenanceRecord data in SQL Server
// It implements the IMaintenanceRecordRepository interface and provides each method declared in that interface.
public class MaintenanceRecordRepository : IMaintenanceRecordRepository
{
    private readonly ToughGuyAutoDbContext _context;

    public MaintenanceRecordRepository(ToughGuyAutoDbContext context)
    {
        _context = context;
    }

    // Looking for all maintenance records.
    // AsNoTracking improves read-only query performance
    public async Task<List<MaintenanceRecord>> GetAllAsync()
    {
        return await _context.MaintenanceRecords
            .Include(m => m.Vehicle)
            .Include(m => m.ServiceTypes)
            .AsNoTracking()
            .ToListAsync();
    }

    // Logic:
    // 1. Start with all maintenance records.
    // 2. Load the related vehicle and service types.
    // 3. Use Where to keep only records whose vehicle belongs to the specified user.
    // 4. Return the results as a list.
    public async Task<List<MaintenanceRecord>> GetByUserIdAsync(string userId)
    {
        return await _context.MaintenanceRecords
            .Include(m => m.Vehicle)
            .Include(m => m.ServiceTypes)
            .Where(m => m.Vehicle.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    // Retrieve one record using its primary key. tracking is kept.
    public async Task<MaintenanceRecord?> GetByIdAsync(int id)
    {
        return await _context.MaintenanceRecords
            .Include(m => m.Vehicle)
            .Include(m => m.ServiceTypes)
            .FirstOrDefaultAsync(
                m => m.MaintenanceRecordId == id);
    }

    // Add marks the entity as Added.
    // SaveChangesAsync generates and executes the SQL INSERT.
    public async Task AddAsync(MaintenanceRecord record)
    {
        _context.MaintenanceRecords.Add(record);
        await _context.SaveChangesAsync();
    }

    // Update marks the entity as Modified
    public async Task UpdateAsync(MaintenanceRecord record)
    {
        _context.MaintenanceRecords.Update(record);
        await _context.SaveChangesAsync();
    }

    // Find the record first because the ID may not exist
    // If it exists, Remove marks it as Deleted and SaveChangesAsync executes the SQL DELETE
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
