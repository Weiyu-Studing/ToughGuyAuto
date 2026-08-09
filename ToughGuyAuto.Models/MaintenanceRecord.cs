using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToughGuyAuto.Models
{
    public class MaintenanceRecord
    {
        public int MaintenanceRecordId { get; set; }

        public int VehicleId { get; set; }

        public DateTime ServiceDate { get; set; }

        public int Mileage { get; set; }

        public decimal Cost { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public Vehicle Vehicle { get; set; } = null!;

        public ICollection<ServiceType> ServiceTypes { get; set; }
            = new List<ServiceType>();
    }
}
