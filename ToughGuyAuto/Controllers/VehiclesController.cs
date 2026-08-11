using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToughGuyAuto.BLL.Interfaces;
using ToughGuyAuto.Models;
using ToughGuyAuto.ViewModels;

namespace ToughGuyAuto.Controllers;

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

    public async Task<IActionResult> Details(int id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        if (!User.IsInRole("Admin"))
        {
            var userId = _userManager.GetUserId(User);

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    [FromForm] VehicleCreateViewModel model1) // there is a trick: don't name model too eazy
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [FromForm] VehicleEditViewModel model1)
    {
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

    // Delete
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

        await _vehicleService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }

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