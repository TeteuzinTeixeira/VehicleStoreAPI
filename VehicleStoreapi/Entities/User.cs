using Microsoft.AspNetCore.Identity;

namespace VehicleStoreapi.Database;

public class User : IdentityUser
{
    public string Id { get; set; }
    public string? Initials { get; set; }
}