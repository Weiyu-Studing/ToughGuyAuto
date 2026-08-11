using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Interfaces;

// This repository interface defines the database operations available for Vehicle entities.
/*The BLL service depends on this interface instead of directly depending on 
    VehicleRepository or ToughGuyAutoDbContext. */
public interface IVehicleRepository
{
    Task<List<Vehicle>> GetAllAsync();

    Task<List<Vehicle>> GetByUserIdAsync(string userId);

    Task<Vehicle?> GetByIdAsync(int id);

    Task AddAsync(Vehicle vehicle);

    Task UpdateAsync(Vehicle vehicle);

    Task DeleteAsync(int id);
}
