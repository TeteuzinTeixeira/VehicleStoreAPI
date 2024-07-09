namespace VehicleStoreapi.Database.Vehicle;

public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public string Addres { get; set; }
}