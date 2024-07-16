using VehicleStoreapi.Database.Vehicle;

namespace VehicleStoreapi.Service;

public interface IOrderService
{
    public Task<bool> CheckIfVehicleExists(Guid vehicleId);
    public Task<bool> CheckIfVehicleAlreadyLinked(Guid vehicleId);
    public Task AddOrder(Order order);
    public Task LinkOrderToVehicle(Guid orderId, Guid vehicleId);
}