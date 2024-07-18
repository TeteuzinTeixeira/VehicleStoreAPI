using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities.Dto;

namespace VehicleStoreapi.Service;

public interface IVehicleService
{
    public bool ValidarVehicle(Vehicle vehicle);
    public Task<Vehicle> SaveVehicle(Vehicle vehicle, List<IFormFile> files);
    public Task<List<VehicleImage>> SaveImages(Vehicle vehicle, List<IFormFile> files);
    Task<bool> UpdateVehicleAsync(Guid id, Vehicle vehicle);
    bool VehicleExists(Guid id);
    Task<bool> UpdateVehicleImagesAsync(Guid vehicleId, List<Guid>? removedImageIds, List<IFormFile> files);
    Task<VehicleDto?> GetVehicleByIdAsync(Guid id);
    Task<List<VehicleImageDto>> GetImagesByVehicleIdAsync(Guid id);
    public Task<List<VehicleWithImagesDto>> GetVehiclesAndImagesAsync();
}