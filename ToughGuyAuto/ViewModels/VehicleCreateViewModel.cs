using System.ComponentModel.DataAnnotations;

namespace ToughGuyAuto.ViewModels;

public class VehicleCreateViewModel
{
    [Required]
    [StringLength(50)]
    public string Make { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Model { get; set; } = string.Empty;

    [Required]
    [Range(1886, 2100)]
    public int Year { get; set; }

    [Required]
    [StringLength(20)]
    public string LicensePlate { get; set; } = string.Empty;

    [Required]
    [StringLength(17, MinimumLength = 17)]
    public string VIN { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue)]
    public int Mileage { get; set; }
}