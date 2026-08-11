using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Interfaces;

// This interface defines the database operations for ServiceType entities.
public interface IServiceTypeRepository
{
    Task<List<ServiceType>> GetAllAsync();

    Task<ServiceType?> GetByIdAsync(int id);

    Task AddAsync(ServiceType serviceType);

    Task UpdateAsync(ServiceType serviceType);

    Task DeleteAsync(int id);
}
