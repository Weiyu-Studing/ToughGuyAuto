using Microsoft.EntityFrameworkCore;
using ToughGuyAuto.DAL.Data;
using ToughGuyAuto.DAL.Interfaces;
using ToughGuyAuto.Models;

namespace ToughGuyAuto.DAL.Repositories;

public class ServiceTypeRepository : IServiceTypeRepository
{
    private readonly ToughGuyAutoDbContext _context;

    public ServiceTypeRepository(ToughGuyAutoDbContext context)
    {
        _context = context;
    }

    public async Task<List<ServiceType>> GetAllAsync()
    {
        return await _context.ServiceTypes
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ServiceType?> GetByIdAsync(int id)
    {
        return await _context.ServiceTypes
            .FirstOrDefaultAsync(
                s => s.ServiceTypeId == id);
    }

    public async Task AddAsync(ServiceType serviceType)
    {
        _context.ServiceTypes.Add(serviceType);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ServiceType serviceType)
    {
        _context.ServiceTypes.Update(serviceType);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var serviceType = await _context.ServiceTypes
            .FirstOrDefaultAsync(
                s => s.ServiceTypeId == id);

        if (serviceType != null)
        {
            _context.ServiceTypes.Remove(serviceType);
            await _context.SaveChangesAsync();
        }
    }
}
