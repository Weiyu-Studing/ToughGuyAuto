using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToughGuyAuto.BLL.Interfaces;
using ToughGuyAuto.Models;

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

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vehicle vehicle)
    {
        if (!ModelState.IsValid)
        {
            return View(vehicle);
        }

        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Challenge();
        }

        vehicle.UserId = userId;

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        Vehicle vehicle)
    {
        if (id != vehicle.VehicleId)
        {
            return NotFound();
        }

        var existingVehicle =
            await _vehicleService.GetByIdAsync(id);

        if (existingVehicle == null)
        {
            return NotFound();
        }

        if (!User.IsInRole("Admin"))
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null ||
                existingVehicle.UserId != userId)
            {
                return Forbid();
            }

            vehicle.UserId = userId;
        }
        else
        {
            vehicle.UserId = existingVehicle.UserId;
        }

        if (!ModelState.IsValid)
        {
            return View(vehicle);
        }

        await _vehicleService.UpdateAsync(vehicle);

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _vehicleService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}


