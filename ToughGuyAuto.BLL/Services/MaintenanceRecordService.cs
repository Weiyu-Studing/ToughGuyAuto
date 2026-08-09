using ToughGuyAuto.BLL.Interfaces;
using ToughGuyAuto.DAL.Interfaces;
using ToughGuyAuto.Models;

namespace ToughGuyAuto.BLL.Services;

public class MaintenanceRecordService
    : IMaintenanceRecordService
{
    private readonly IMaintenanceRecordRepository _repository;

    public MaintenanceRecordService(
        IMaintenanceRecordRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MaintenanceRecord>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<List<MaintenanceRecord>> GetUserRecordsAsync(
        string userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<MaintenanceRecord?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> CanUserAccessAsync(
        int recordId,
        string userId)
    {
        var record = await _repository.GetByIdAsync(recordId);

        if (record == null || record.Vehicle == null)
        {
            return false;
        }

        return record.Vehicle.UserId == userId;
    }

    public async Task CreateAsync(MaintenanceRecord record)
    {
        if (record.Mileage < 0)
        {
            throw new ArgumentException(
                "Mileage cannot be negative.");
        }

        if (record.Cost < 0)
        {
            throw new ArgumentException(
                "Cost cannot be negative.");
        }

        await _repository.AddAsync(record);
    }

    public async Task UpdateAsync(MaintenanceRecord record)
    {
        if (record.Mileage < 0)
        {
            throw new ArgumentException(
                "Mileage cannot be negative.");
        }

        if (record.Cost < 0)
        {
            throw new ArgumentException(
                "Cost cannot be negative.");
        }

        await _repository.UpdateAsync(record);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
