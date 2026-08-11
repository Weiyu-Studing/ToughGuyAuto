using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToughGuyAuto.BLL.Interfaces;
using ToughGuyAuto.Models;

namespace ToughGuyAuto.Controllers;

[Authorize(Roles = "Admin")]
public class ServiceTypesController : Controller
{
    private readonly IServiceTypeService _serviceTypeService;

    public ServiceTypesController(
        IServiceTypeService serviceTypeService)
    {
        _serviceTypeService = serviceTypeService;
    }

    // Show all service types
    public async Task<IActionResult> Index()
    {
        var serviceTypes =
            await _serviceTypeService.GetAllAsync();

        return View(serviceTypes);
    }

    // Show details
    public async Task<IActionResult> Details(int id)
    {
        var serviceType =
            await _serviceTypeService.GetByIdAsync(id);

        if (serviceType == null)
        {
            return NotFound();
        }

        return View(serviceType);
    }

    // Show create page
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ServiceType serviceType)
    {
        if (!ModelState.IsValid)
        {
            return View(serviceType);
        }

        try
        {
            await _serviceTypeService.CreateAsync(serviceType);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);

            return View(serviceType);
        }

        return RedirectToAction(nameof(Index));
    }

    // Show edit page
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var serviceType =
            await _serviceTypeService.GetByIdAsync(id);

        if (serviceType == null)
        {
            return NotFound();
        }

        return View(serviceType);
    }

    // Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ServiceType serviceType)
    {
        if (id != serviceType.ServiceTypeId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(serviceType);
        }

        try
        {
            await _serviceTypeService.UpdateAsync(serviceType);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);

            return View(serviceType);
        }

        return RedirectToAction(nameof(Index));
    }

    // Show delete page
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var serviceType =
            await _serviceTypeService.GetByIdAsync(id);

        if (serviceType == null)
        {
            return NotFound();
        }

        return View(serviceType);
    }

    // Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _serviceTypeService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}
