namespace VehicleStoreapi.Database.Vehicle;

public class VehicleImage
{
    public Guid Id { get; set; }
    public Vehicle Vehicle { get; set; }
    public Guid VehicleId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public byte Data { get; set; }
}