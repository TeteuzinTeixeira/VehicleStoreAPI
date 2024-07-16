using Microsoft.EntityFrameworkCore;
using VehicleStoreapi.Database;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities;

namespace VehicleStoreapi.Service.Impl;

public class OrderServiceImpl : IOrderService
{
    private readonly AppDbContext _context;

    public OrderServiceImpl(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> CheckIfVehicleExists(Guid vehicleId)
    {
        var dbVehicle = await _context.Vehicle.FindAsync(vehicleId);
        return dbVehicle != null;
    }

    public async Task<bool> CheckIfVehicleAlreadyLinked(Guid vehicleId)
    {
        var existingOrderVehicleLink = await _context.OrderVehicleLink
            .FirstOrDefaultAsync(ovl => ovl.VehicleId == vehicleId);
        return existingOrderVehicleLink != null;
    }

    public async Task AddOrder(Order order)
    {
        _context.Order.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task LinkOrderToVehicle(Guid orderId, Guid vehicleId)
    {
        var orderVehicleLink = new OrderVehicleLink
        {
            OrderId = orderId,
            VehicleId = vehicleId
        };

        _context.OrderVehicleLink.Add(orderVehicleLink);
        await _context.SaveChangesAsync();
    }
}