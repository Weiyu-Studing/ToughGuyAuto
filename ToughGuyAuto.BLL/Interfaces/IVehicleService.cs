using ToughGuyAuto.Models;

namespace ToughGuyAuto.BLL.Interfaces;

public interface IVehicleService
{
    Task<List<Vehicle>> GetAllAsync();

    Task<List<Vehicle>> GetUserVehiclesAsync(string userId);

    Task<Vehicle?> GetByIdAsync(int id);

    Task<bool> CanUserAccessAsync(
        int vehicleId,
        string userId);

    Task CreateAsync(Vehicle vehicle);

    Task UpdateAsync(Vehicle vehicle);

    Task DeleteAsync(int id);
}