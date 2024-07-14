using VehicleStoreapi.Database.Vehicle;

namespace VehicleStoreapi.Service;

public interface VehicleService
{
    public bool ValidarVehicle(Vehicle vehicle);

    public Task<Vehicle> SaveVehicle(Vehicle vehicle, List<IFormFile> files);
    
    public Task<List<VehicleImage>> SaveImages(Vehicle vehicle, List<IFormFile> files);
}