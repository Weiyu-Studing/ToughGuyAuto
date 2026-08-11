using System.ComponentModel.DataAnnotations;

namespace ToughGuyAuto.ViewModels;

public class MaintenanceRecordCreateViewModel
{
    // Identify the vehicle that received the maintenance service.
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

    // For example, if the Admin selects Oil Change and Tire Rotation, 
    // this list stores the IDs of those two ServiceType records.

    // The Controller uses these IDs to retrieve the ServiceType entities and add them to MaintenanceRecord.ServiceTypes.
    public List<int> SelectedServiceTypeIds { get; set; }
        = new List<int>();
}
