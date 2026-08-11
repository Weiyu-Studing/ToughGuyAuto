using ToughGuyAuto.BLL.Interfaces;
using ToughGuyAuto.DAL.Interfaces;
using ToughGuyAuto.Models;

namespace ToughGuyAuto.BLL.Services;

public class ServiceTypeService : IServiceTypeService
{
    private readonly IServiceTypeRepository _repository;

    public ServiceTypeService(
        IServiceTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ServiceType>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ServiceType?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    // A service type can't be empty.
    // IsNullOrWhiteSpace rejects null, empty text and spaces-only text.
    public async Task CreateAsync(ServiceType serviceType)
    {
        if (string.IsNullOrWhiteSpace(serviceType.Name))
        {
            throw new ArgumentException(
                "Service type name is required.");
        }

        await _repository.AddAsync(serviceType);
    }

    public async Task UpdateAsync(ServiceType serviceType)
    {
        if (string.IsNullOrWhiteSpace(serviceType.Name))
        {
            throw new ArgumentException(
                "Service type name is required.");
        }

        await _repository.UpdateAsync(serviceType);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
