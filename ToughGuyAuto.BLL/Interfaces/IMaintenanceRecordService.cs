using ToughGuyAuto.Models;

namespace ToughGuyAuto.BLL.Interfaces;

public interface IMaintenanceRecordService
{
    Task<List<MaintenanceRecord>> GetAllAsync();

    Task<List<MaintenanceRecord>> GetUserRecordsAsync(
        string userId);

    Task<MaintenanceRecord?> GetByIdAsync(int id);

    Task<bool> CanUserAccessAsync(
        int recordId,
        string userId);

    Task CreateAsync(MaintenanceRecord record);

    Task UpdateAsync(MaintenanceRecord record);

    Task DeleteAsync(int id);
}
