using ToughGuyAuto.BLL.Interfaces;
using ToughGuyAuto.DAL.Interfaces;
using ToughGuyAuto.Models;

namespace ToughGuyAuto.BLL.Services;

// This service contains the business logic for maintenance records.
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

    // Check ownership through the related Vehicle.
    // user's ID must be checked through record.Vehicle.UserId.
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

    // Mileage and Cost cannot be negative
    // The Controller adds the message to ModelState for the View.
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

    // Apply the same business rules when updating an existing record
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
