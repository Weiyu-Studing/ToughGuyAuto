namespace ToughGuyAuto.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string Make { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int Year { get; set; }

        public string LicensePlate { get; set; } = string.Empty;

        public string VIN { get; set; } = string.Empty;

        public int Mileage { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; }
            = new List<MaintenanceRecord>();
    }
}
