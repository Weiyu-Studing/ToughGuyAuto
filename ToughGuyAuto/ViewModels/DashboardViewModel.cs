namespace ToughGuyAuto.ViewModels;

// This ViewModel combines summary information from different entities for the Dashboard page.
// It displays calculated information such as totals, costs and recent maintenance activity.
public class DashboardViewModel
{
    public int TotalVehicles { get; set; }

    public int TotalMaintenanceRecords { get; set; }

    public decimal TotalMaintenanceCost { get; set; }

    public string? MostUsedServiceType { get; set; }

    public List<RecentMaintenanceViewModel> RecentMaintenance
    { get; set; } = new();
}

// This smaller ViewModel represents one recent maintenance item.
// It contains only the information required by the Dashboard.
public class RecentMaintenanceViewModel
{
    public string VehicleName { get; set; } = string.Empty;

    public DateTime ServiceDate { get; set; }

    public decimal Cost { get; set; }

    public List<string> ServiceTypes { get; set; }
        = new();
}
