using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Interfaces;

public interface IVehicleRepository
{
    Task<List<Vehicle>> GetAllAsync();

    Task<List<Vehicle>> GetByUserIdAsync(string userId);

    Task<Vehicle?> GetByIdAsync(int id);

    Task AddAsync(Vehicle vehicle);

    Task UpdateAsync(Vehicle vehicle);

    Task DeleteAsync(int id);
}
