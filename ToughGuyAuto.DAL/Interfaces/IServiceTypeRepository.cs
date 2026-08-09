using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Interfaces;

public interface IServiceTypeRepository
{
    Task<List<ServiceType>> GetAllAsync();

    Task<ServiceType?> GetByIdAsync(int id);

    Task AddAsync(ServiceType serviceType);

    Task UpdateAsync(ServiceType serviceType);

    Task DeleteAsync(int id);
}
