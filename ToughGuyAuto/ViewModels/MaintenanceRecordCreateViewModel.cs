using System.ComponentModel.DataAnnotations;

namespace ToughGuyAuto.ViewModels;

public class MaintenanceRecordCreateViewModel
{
    [Required]
    public int VehicleId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime ServiceDate { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Mileage { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Cost { get; set; }

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Notes { get; set; } = string.Empty;

    public List<int> SelectedServiceTypeIds { get; set; }
        = new List<int>();
}
