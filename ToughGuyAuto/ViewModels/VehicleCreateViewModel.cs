namespace ToughGuyAuto.ViewModels;

public class VehicleCreateViewModel
{
    public string Make { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public string LicensePlate { get; set; } = string.Empty;

    public string VIN { get; set; } = string.Empty;

    public int Mileage { get; set; }
}
