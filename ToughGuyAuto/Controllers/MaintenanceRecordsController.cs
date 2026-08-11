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

    // IMaintenanceRecordService manages maintenance record operations.
    // IVehicleService supplies vehicle choices.
    // IServiceTypeService supplies service type choices.
    // UserManager identifies the currently logged-in user.
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

    // Logic:
    // 1. Get the current user's Identity ID.
    // 2. An Admin can retrieve all maintenance records.
    // 3. A regular user can retrieve only records connected to vehicles that they own.
    // 4. Send the correct list to the Index View.
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

    // Logic:
    // 1. Retrieve the maintenance record and its related data.
    // 2. Return HTTP 404 if it does not exist.
    // 3. Allow an Admin to view any record.
    // 4. For a regular user, compare record.Vehicle.UserId with the current user's ID.
    // 5. Return HTTP 403 if the user does not own the related vehicle.
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
    // only an Admin can submit this form.
    // Logic:
    // 1. Validate the submitted ViewModel.
    // 2. Reload ViewBag data if validation fails.
    // 3. Convert the ViewModel into a MaintenanceRecord entity.
    // 4. Retrieve the selected ServiceType entities.
    // 5. Add them to the many-to-many navigation collection.
    // 6. Ask the service to validate and save the record.
    // 7. Redirect to Index after a successful save.
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

        // Convert the validated form data into a database entity.
        // SelectedServiceTypeIds is handled separately because it represents a many-to-many relationship.
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

    // Logic:
    // 1. Retrieve the existing record and related ServiceTypes.
    // 2. Return HTTP 404 if it does not exist.
    // 3. Check access permission.
    // 4. Convert the entity into an Edit ViewModel.
    // 5. Copy the currently selected service type IDs into the ViewModel.
    // 6. Load all vehicles and service types for the form.
    // 7. Display the Edit View.
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

    // Logic:
    // 1. Confirm that the URL ID matches the hidden form ID.
    // 2. Check ModelState validation.
    // 3. Retrieve the existing record.
    // 4. Check permission.
    // 5. Copy the ViewModel values into the entity.
    // 6. Replace the old many-to-many selections.
    // 7. Ask the service to validate and update the record.
    // 8. Redirect to Index after a successful update.
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

        // Remove the old many-to-many selections.
        // The following loop adds the service types currently selected in the Edit form.
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

    // Deleting the record also removes its rows from the MaintenanceRecordServiceTypes join table.
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

    // An Admin can access every maintenance record.
    // A regular user can access a record only when they own the vehicle connected to that record.
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

    /* This helper avoids repeating the same vehicle and service type
       queries when first displaying the form or redisplaying it after validation fails. */
    private async Task LoadCreateData()
    {
        ViewBag.Vehicles =
            await _vehicleService.GetAllAsync();

        ViewBag.ServiceTypes =
            await _serviceTypeService.GetAllAsync();
    }
}
