using ToughGuyAuto.Models;

namespace ToughGuyAuto.BLL.Interfaces;

/*This interface defines the operations available to Controllers for working with maintenance records.
  The Controller only knows what operations are available.
  MaintenanceRecordService contains the actual business logic. */
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
