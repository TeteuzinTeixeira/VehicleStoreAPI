namespace VehicleStoreapi.Model.Entities;

public class VehicleUploadDTO
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string Model { get; set; }
    public string Type { get; set; }
    public string Year { get; set; }
    public double Value { get; set; }
}