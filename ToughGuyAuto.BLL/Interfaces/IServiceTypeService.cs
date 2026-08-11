using ToughGuyAuto.Models;

namespace ToughGuyAuto.BLL.Interfaces;

// ServiceTypesController calls these methods without needing to know how ServiceTypeService performs the work.
public interface IServiceTypeService
{
    Task<List<ServiceType>> GetAllAsync();

    Task<ServiceType?> GetByIdAsync(int id);

    Task CreateAsync(ServiceType serviceType);

    Task UpdateAsync(ServiceType serviceType);

    Task DeleteAsync(int id);
}