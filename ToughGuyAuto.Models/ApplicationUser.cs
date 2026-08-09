using Microsoft.AspNetCore.Identity;

namespace ToughGuyAuto.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Vehicle> Vehicles { get; set; }
        = new List<Vehicle>();
}