using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleStoreapi.AutoMapper;
using VehicleStoreapi.Database;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities;
using VehicleStoreapi.Model.Entities.Dto;
using VehicleStoreapi.Service;

namespace VehicleStoreapi.Controller;

[Authorize]
[Route("[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IOrderService _service;
    private readonly IMapper _mapper;

    public OrderController(AppDbContext context, IOrderService service, IMapper mapper)
    {
        _context = context;
        _service = service;
        _mapper = mapper;
    }
    
    [HttpPost("CreateOrder")]
    public async Task<ActionResult<OrderDto>> CreateOrder(Order order, Guid vehicleId)
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
        
        var orderDto = _mapper.Map<OrderDto>(order);

        return Ok(orderDto);
    }
    
    [HttpDelete("Delete/{vehicleId:guid}")]
    public async Task<ActionResult> DeleteOrder(Guid vehicleId)
    {
        var dbProduct = await _context.OrderVehicleLink
            .FirstOrDefaultAsync(ovl => ovl.VehicleId == vehicleId);

        if (dbProduct == null)
        {
            return NotFound($"Nenhum pedido encontrado para o vehicle id: {vehicleId}");
        }

        _context.OrderVehicleLink.Remove(dbProduct);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}