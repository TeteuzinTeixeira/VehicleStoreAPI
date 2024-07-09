using VehicleStoreapi.Database;
using VehicleStoreapi.Database.Vehicle;

namespace VehicleStoreapi.Model;

public class VehicleDTO
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public User? User { get; set; }
    public Order? Order { get; set; }
    public Guid? OrderId { get; set; }
    public string Model { get; set; }
    public string Type { get; set; }
    public string Year { get; set; }
    public double Value { get; set; }
}