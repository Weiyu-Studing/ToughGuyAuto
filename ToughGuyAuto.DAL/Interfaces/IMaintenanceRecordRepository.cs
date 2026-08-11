using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Interfaces;

// This interface defines the database operations for MaintenanceRecord entities.
public interface IMaintenanceRecordRepository
{
    Task<List<MaintenanceRecord>> GetAllAsync();

    Task<List<MaintenanceRecord>> GetByUserIdAsync(
        string userId);

    Task<MaintenanceRecord?> GetByIdAsync(int id);

    Task AddAsync(MaintenanceRecord record);

    Task UpdateAsync(MaintenanceRecord record);

    Task DeleteAsync(int id);
}
