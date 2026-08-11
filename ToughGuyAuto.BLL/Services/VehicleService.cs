using ToughGuyAuto.BLL.Interfaces;
using ToughGuyAuto.DAL.Interfaces;
using ToughGuyAuto.Models;

namespace ToughGuyAuto.BLL.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _repository;

    public VehicleService(IVehicleRepository repository)
    {
        _repository = repository;
    }

    // Ask the repository to retrieve every vehicle.
    public async Task<List<Vehicle>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    // Retrieve only vehicles that belong to the specified user
    public async Task<List<Vehicle>> GetUserVehiclesAsync(
        string userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    // Retrieve one vehicle by its PK
    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    // Logic:
    // 1. Retrieve the requested vehicle.
    // 2. Return false if it does not exist.
    // 3. Compare the vehicle's UserId with the current user's ID.
    public async Task<bool> CanUserAccessAsync(
        int vehicleId,
        string userId)
    {
        var vehicle = await _repository.GetByIdAsync(vehicleId);

        if (vehicle == null)
        {
            return false;
        }

        return vehicle.UserId == userId;
    }

    // validations
    public async Task CreateAsync(Vehicle vehicle)
    {
        if (string.IsNullOrWhiteSpace(vehicle.Make))
        {
            throw new ArgumentException("Make is required.");
        }

        if (string.IsNullOrWhiteSpace(vehicle.Model))
        {
            throw new ArgumentException("Model is required.");
        }

        if (vehicle.Mileage < 0)
        {
            throw new ArgumentException(
                "Mileage cannot be negative.");
        }

        await _repository.AddAsync(vehicle);
    }

    // Validate the updated vehicle before sending it to the DAL
    public async Task UpdateAsync(Vehicle vehicle)
    {
        if (vehicle.Mileage < 0)
        {
            throw new ArgumentException(
                "Mileage cannot be negative.");
        }

        await _repository.UpdateAsync(vehicle);
    }

    // The service requests deletion
    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
