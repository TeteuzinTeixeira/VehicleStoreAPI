using VehicleStoreapi.Database;

namespace VehicleStoreapi.Model.Entities.Dto;

public class OrderDto
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public User?  User { get; set; }
    public string Addres { get; set; }
}