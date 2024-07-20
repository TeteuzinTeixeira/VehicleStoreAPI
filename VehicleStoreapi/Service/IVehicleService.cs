using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities.Dto;

namespace VehicleStoreapi.Service;

public interface IVehicleService
{
    public Task<Vehicle> SaveVehicle(Vehicle vehicle, List<IFormFile> files);
    Task<bool> UpdateVehicleAsync(Guid id, Vehicle vehicle);
    Task<bool> UpdateVehicleImagesAsync(Guid vehicleId, List<Guid>? removedImageIds, List<IFormFile> files);
    Task<VehicleDto?> GetVehicleByIdAsync(Guid id);
    Task<List<VehicleImageDto>> GetImagesByVehicleIdAsync(Guid id);
    public Task<List<VehicleWithImagesDto>> GetVehiclesAndImagesAsync();
}