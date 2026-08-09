namespace ToughGuyAuto.ViewModels;

public class DashboardViewModel
{
    public int TotalVehicles { get; set; }

    public int TotalMaintenanceRecords { get; set; }

    public decimal TotalMaintenanceCost { get; set; }

    public string? MostUsedServiceType { get; set; }

    public List<RecentMaintenanceViewModel> RecentMaintenance
    { get; set; } = new();
}

public class RecentMaintenanceViewModel
{
    public string VehicleName { get; set; } = string.Empty;

    public DateTime ServiceDate { get; set; }

    public decimal Cost { get; set; }

    public List<string> ServiceTypes { get; set; }
        = new();
}
