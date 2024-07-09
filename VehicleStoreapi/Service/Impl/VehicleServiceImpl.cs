using VehicleStoreapi.Database.Vehicle;

namespace VehicleStoreapi.Service.Impl;

public class VehicleServiceImpl : VehicleService
{
    public bool ValidarVehicle(Vehicle vehicle)
    {
        if (vehicle.Equals(null))
        {
            return false;
        }
        
        if (vehicle.Value <= 0.0)
        {
            return false;
        }

        return true;
    }
}