using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToughGuyAuto.BLL.Interfaces;
using ToughGuyAuto.Models;
using ToughGuyAuto.ViewModels;

namespace ToughGuyAuto.Controllers;

// This is responsible for:
// 1. Checking authentication and authorization.
// 2. Receiving and validating form data.
// 3. Converting ViewModels into Vehicle entities.
// 4. Calling IVehicleService to perform business operations.
// 5. Returning a View, redirect or HTTP error response.
[Authorize]
public class VehiclesController : Controller
{
    private readonly IVehicleService _vehicleService;
    private readonly UserManager<ApplicationUser> _userManager;

    public VehiclesController(
        IVehicleService vehicleService,
        UserManager<ApplicationUser> userManager)
    {
        _vehicleService = vehicleService;
        _userManager = userManager;
    }

    // Logic:
    // 1. Check whether the current user is an Admin.
    // 2. If the user is an Admin, retrieve all vehicles.
    // 3. Otherwise, get the current user's Identity ID.
    // 4. Retrieve only vehicles owned by that user.
    // 5. Send the correct vehicle list to the Index View.
    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Admin"))
        {
            var vehicles = await _vehicleService.GetAllAsync();
            return View(vehicles);
        }

        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        var userVehicles =
            await _vehicleService.GetUserVehiclesAsync(userId);

        return View(userVehicles);
    }

    // Logic:
    // 1. Retrieve the vehicle by its primary key.
    // 2. Return HTTP 404 if the vehicle does not exist.
    // 3. Allow an Admin to view any vehicle.
    // 4. For a regular user, compare the vehicle's UserId
    //    with the current user's Identity ID.
    // 5. Return HTTP 403 if the vehicle belongs to another user.
    // 6. Send the vehicle to the Details View.
    public async Task<IActionResult> Details(int id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);

        // NotFound returns HTTP 404 because the requested
        // vehicle does not exist.
        if (vehicle == null)
        {
            return NotFound();
        }

        if (!User.IsInRole("Admin"))
        {
            var userId = _userManager.GetUserId(User);

            // Forbid returns HTTP 403 when the user is logged in
            // but does not have permission to access this vehicle.
            if (userId == null ||
                vehicle.UserId != userId)
            {
                return Forbid();
            }
        }
        return View(vehicle);
    }


    public IActionResult Create()
    {
        return View();
    }


    // Logic:
    // 1. Check the Data Annotation validation results.
    // 2. Get the current user's Identity ID.
    // 3. Convert the ViewModel into a Vehicle entity.
    // 4. Assign the current user as the vehicle owner.
    // 5. Ask the service to validate and save the vehicle.
    // 6. Display a business validation error if saving fails.
    // 7. Redirect to Index after a successful save.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    [FromForm] VehicleCreateViewModel model1) // there is a trick: don't name model too eazy ;)
    {
        if (!ModelState.IsValid)
        {
            return View(model1);
        }

        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        // Convert the validated ViewModel into a Vehicle entity.
        var vehicle = new Vehicle
        {
            UserId = userId,
            Make = model1.Make.Trim(),
            Model = model1.Model.Trim(),
            Year = model1.Year,
            LicensePlate = model1.LicensePlate.Trim(),
            VIN = model1.VIN.Trim().ToUpper(),
            Mileage = model1.Mileage
        };

        
        await _vehicleService.CreateAsync(vehicle);

        return RedirectToAction(nameof(Index));
    }

    // Logic:
    // 1. Retrieve the vehicle.
    // 2. Return HTTP 404 if it does not exist.
    // 3. Check whether the current user can access it.
    // 4. Convert the Vehicle entity into a VehicleEditViewModel.
    // 5. Send the ViewModel to the Edit View.
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        if (!await CanAccessVehicle(vehicle))
        {
            return Forbid();
        }

        var model = new VehicleEditViewModel
        {
            VehicleId = vehicle.VehicleId,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            LicensePlate = vehicle.LicensePlate,
            VIN = vehicle.VIN,
            Mileage = vehicle.Mileage
        };

        return View(model);
    }

    // Logic:
    // 1. Confirm that the URL ID matches the submitted form ID.
    // 2. Check ModelState validation.
    // 3. Retrieve the existing Vehicle entity.
    // 4. Check whether the current user can edit it.
    // 5. Copy the ViewModel values into the existing entity.
    // 6. Ask the service to validate and update the vehicle.
    // 7. Redirect to Index after a successful update.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [FromForm] VehicleEditViewModel model1)
    {
        // The URL ID and hidden form ID must identify the same vehicle.
        if (id != model1.VehicleId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model1);
        }

        var vehicle = await _vehicleService.GetByIdAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        if (!await CanAccessVehicle(vehicle))
        {
            return Forbid();
        }

        vehicle.Make = model1.Make.Trim();
        vehicle.Model = model1.Model.Trim();
        vehicle.Year = model1.Year;
        vehicle.LicensePlate = model1.LicensePlate.Trim();
        vehicle.VIN = model1.VIN.Trim().ToUpper();
        vehicle.Mileage = model1.Mileage;

        try
        {
            await _vehicleService.UpdateAsync(vehicle);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            return View(model1);
        }

        return RedirectToAction(nameof(Index));
    }

    // Logic:
    // 1. Retrieve the vehicle.
    // 2. Return HTTP 404 if it does not exist.
    // 3. Check whether the current user can delete it.
    // 4. Send the vehicle to the confirmation View.
    public async Task<IActionResult> Delete(int id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        if (!await CanAccessVehicle(vehicle))
        {
            return Forbid();
        }

        return View(vehicle);
    }

    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        if (!await CanAccessVehicle(vehicle))
        {
            return Forbid();
        }

        // Cascade delete is configured in DbContext.
        // Deleting this vehicle also deletes the maintenance records that belong to this vehicle.
        await _vehicleService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }

    // This helper method keeps the vehicle access rule
    // An Admin can access every vehicle.
    // A regular user can access only a vehicle whose UserId
    // matches the currently logged-in user's Identity ID.
    private async Task<bool> CanAccessVehicle(
        Vehicle vehicle)
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

        return vehicle.UserId == userId;
    }
}