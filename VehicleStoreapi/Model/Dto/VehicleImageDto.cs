namespace VehicleStoreapi.Model.Entities.Dto;

public class VehicleImageDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string Path { get; set; }
}