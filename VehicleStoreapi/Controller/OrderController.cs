using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleStoreapi.Database;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities;
using VehicleStoreapi.Service;
using VehicleStoreapi.Service.Impl;

namespace VehicleStoreapi.Controller;

[Authorize]
[Route("[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IOrderService _service;

    public OrderController(AppDbContext context, IOrderService service)
    {
        _context = context;
        _service = service;
    }
    
    [HttpPost("CreateOrder")]
    public async Task<ActionResult<Order>> CreateOrder(Order order, Guid vehicleId)
    {
        order.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!await _service.CheckIfVehicleExists(vehicleId))
        {
            return NotFound("Vehicle não encontrado");
        }

        if (await _service.CheckIfVehicleAlreadyLinked(vehicleId))
        {
            return BadRequest("Este Vehicle encontra-se indisponivel para compra.");
        }

        await _service.AddOrder(order);
        await _service.LinkOrderToVehicle(order.Id, vehicleId);

        return Ok(order);
    }
}