namespace VehicleStoreapi.Model.Entities.Dto;

public class UpdateVehicleImagesDto
{
    public List<Guid> RemovedImageIds { get; set; }
    public List<NewImageDto> NewImages { get; set; }
}