using VehicleStoreapi.Database.Vehicle;

namespace VehicleStoreapi.Model.Entities;

public class OrderVehicleLink
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public Guid OrderId { get; set; }
}