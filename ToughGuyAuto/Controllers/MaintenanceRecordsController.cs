using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToughGuyAuto.BLL.Interfaces;
using ToughGuyAuto.Models;
using ToughGuyAuto.ViewModels;

namespace ToughGuyAuto.Controllers;

[Authorize]
public class MaintenanceRecordsController : Controller
{
    private readonly IMaintenanceRecordService _maintenanceService;
    private readonly IVehicleService _vehicleService;
    private readonly IServiceTypeService _serviceTypeService;
    private readonly UserManager<ApplicationUser> _userManager;

    public MaintenanceRecordsController(
        IMaintenanceRecordService maintenanceService,
        IVehicleService vehicleService,
        IServiceTypeService serviceTypeService,
        UserManager<ApplicationUser> userManager)
    {
        _maintenanceService = maintenanceService;
        _vehicleService = vehicleService;
        _serviceTypeService = serviceTypeService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        if (User.IsInRole("Admin"))
        {
            var records = await _maintenanceService.GetAllAsync();

            return View(records);
        }

        var userRecords =
            await _maintenanceService.GetUserRecordsAsync(userId);

        return View(userRecords);
    }

    public async Task<IActionResult> Details(int id)
    {
        var record =
            await _maintenanceService.GetByIdAsync(id);

        if (record == null)
        {
            return NotFound();
        }

        if (!User.IsInRole("Admin"))
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null ||
                record.Vehicle.UserId != userId)
            {
                return Forbid();
            }
        }

        return View(record);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Vehicles =
            await _vehicleService.GetAllAsync();

        ViewBag.ServiceTypes =
            await _serviceTypeService.GetAllAsync();

        return View();
    }

    // post 
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        MaintenanceRecordCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCreateData();

            return View(model);
        }

        var record = new MaintenanceRecord
        {
            VehicleId = model.VehicleId,
            ServiceDate = model.ServiceDate,
            Mileage = model.Mileage,
            Cost = model.Cost,
            Description = model.Description.Trim(),
            Notes = model.Notes.Trim()
        };

        foreach (var id in model.SelectedServiceTypeIds)
        {
            var serviceType =
                await _serviceTypeService.GetByIdAsync(id);

            if (serviceType != null)
            {
                record.ServiceTypes.Add(serviceType);
            }
        }

        try
        {
            await _maintenanceService.CreateAsync(record);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);

            await LoadCreateData();

            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var record =
            await _maintenanceService.GetByIdAsync(id);

        if (record == null)
        {
            return NotFound();
        }

        if (!await CanAccessRecord(record))
        {
            return Forbid();
        }

        var model = new MaintenanceRecordEditViewModel
        {
            MaintenanceRecordId = record.MaintenanceRecordId,
            VehicleId = record.VehicleId,
            ServiceDate = record.ServiceDate,
            Mileage = record.Mileage,
            Cost = record.Cost,
            Description = record.Description,
            Notes = record.Notes,

            SelectedServiceTypeIds =
                record.ServiceTypes
                    .Select(x => x.ServiceTypeId)
                    .ToList()
        };

        ViewBag.Vehicles =
            await _vehicleService.GetAllAsync();

        ViewBag.ServiceTypes =
            await _serviceTypeService.GetAllAsync();

        return View(model);
    }

    // Edit post
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        MaintenanceRecordEditViewModel model)
    {
        if (id != model.MaintenanceRecordId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Vehicles =
                await _vehicleService.GetAllAsync();

            ViewBag.ServiceTypes =
                await _serviceTypeService.GetAllAsync();

            return View(model);
        }

        var record =
            await _maintenanceService.GetByIdAsync(id);

        if (record == null)
        {
            return NotFound();
        }

        if (!await CanAccessRecord(record))
        {
            return Forbid();
        }

        record.VehicleId = model.VehicleId;
        record.ServiceDate = model.ServiceDate;
        record.Mileage = model.Mileage;
        record.Cost = model.Cost;
        record.Description = model.Description.Trim();
        record.Notes = model.Notes.Trim();

        record.ServiceTypes.Clear();

        foreach (var serviceTypeId in model.SelectedServiceTypeIds)
        {
            var serviceType =
                await _serviceTypeService.GetByIdAsync(serviceTypeId);

            if (serviceType != null)
            {
                record.ServiceTypes.Add(serviceType);
            }
        }

        try
        {
            await _maintenanceService.UpdateAsync(record);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);

            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                ViewBag.Vehicles =
                    await _vehicleService.GetUserVehiclesAsync(userId);
            }

            ViewBag.ServiceTypes =
                await _serviceTypeService.GetAllAsync();

            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var record =
            await _maintenanceService.GetByIdAsync(id);

        if (record == null)
        {
            return NotFound();
        }

        if (!await CanAccessRecord(record))
        {
            return Forbid();
        }

        return View(record);
    }

    //
    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var record =
            await _maintenanceService.GetByIdAsync(id);

        if (record == null)
        {
            return NotFound();
        }

        if (!await CanAccessRecord(record))
        {
            return Forbid();
        }

        await _maintenanceService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> CanAccessRecord(
        MaintenanceRecord record)
    {
        if (User.IsInRole("Admin"))
        {
            return true;
        }

        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return false;
        }

        return record.Vehicle.UserId == userId;
    }

    private async Task LoadCreateData()
    {
        ViewBag.Vehicles =
            await _vehicleService.GetAllAsync();

        ViewBag.ServiceTypes =
            await _serviceTypeService.GetAllAsync();
    }
}
