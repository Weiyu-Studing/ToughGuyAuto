namespace ToughGuyAuto.Models
{
    public class ServiceType
    {
        public int ServiceTypeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; }
            = new List<MaintenanceRecord>();
    }
}
