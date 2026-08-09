using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Interfaces;

public interface IMaintenanceRecordRepository
{
    Task<List<MaintenanceRecord>> GetAllAsync();

    Task<List<MaintenanceRecord>> GetByUserIdAsync(string userId);

    Task<MaintenanceRecord?> GetByIdAsync(int id);

    Task AddAsync(MaintenanceRecord record);

    Task UpdateAsync(MaintenanceRecord record);

    Task DeleteAsync(int id);
}
