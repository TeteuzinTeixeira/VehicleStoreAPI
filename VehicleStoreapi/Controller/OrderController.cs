using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleStoreapi.Database;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities;

namespace VehicleStoreapi.Controller;

[Authorize]
[Route("[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrderController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost("CreateOrder")]
    public async Task<ActionResult<Order>> CreateOrder(Order order, Guid vehicleId)
    {
        order.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var dbVehicle = await _context.Vehicle.FindAsync(vehicleId);
        if (dbVehicle == null)
        {
            return NotFound("Vehicle not found.");
        }
        
        _context.Order.Add(order);
        await _context.SaveChangesAsync();
        
        var orderVehicleLink = new OrderVehicleLink
        {
            OrderId = order.Id,
            VehicleId = vehicleId
        };
        
        _context.OrderVehicleLink.Add(orderVehicleLink);
        await _context.SaveChangesAsync();

        return Ok(order);
    }
}