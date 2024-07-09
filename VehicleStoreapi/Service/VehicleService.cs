using VehicleStoreapi.Database.Vehicle;

namespace VehicleStoreapi.Service;

public interface VehicleService
{
    public bool ValidarVehicle(Vehicle vehicle);
}