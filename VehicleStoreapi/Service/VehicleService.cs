using VehicleStoreapi.Database.Vehicle;

namespace VehicleStoreapi.Service;

public interface VehicleService
{
    public bool ValidarVehicle(Vehicle vehicle);

    public void SalvarImage(VehicleImage vehicleImage);
}