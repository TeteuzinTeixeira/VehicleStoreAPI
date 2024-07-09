using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleStoreapi.Database;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model;
using VehicleStoreapi.Service;

namespace VehicleStoreapi;

[Route("Vehicle/[controller]")]
[ApiController]
public class VehicleController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly VehicleService _service;
    
    public VehicleController(AppDbContext context, VehicleService service)
    {
        _context = context;
        _service = service;
    }
    
    [HttpPost("/Save")]
    public async Task<ActionResult> PostVehicle(Vehicle vehicle)
    {
        if (!_service.ValidarVehicle(vehicle))
            return BadRequest("Preencha todos os campos corretamente");

        _context.Vehicle.Add(vehicle);
        await _context.SaveChangesAsync();
        
        return Ok(vehicle);
    }
    
    [HttpDelete("/Delete/{id}")]
    public async Task<ActionResult> DeleteVehicle(Guid id)
    {
        var dbProduct = await _context.Vehicle.FindAsync(id);

        if (dbProduct == null)
            return NotFound();

        _context.Vehicle.Remove(dbProduct);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpPut("/Update/{id}")]
    public async Task<ActionResult> UpdateProduct(Vehicle vehicle)
    {
        var dbProduct = await _context.Vehicle.FindAsync(vehicle.Id);

        if (dbProduct == null)
            return NotFound();

        dbProduct.Value = vehicle.Value;
        dbProduct.Model = vehicle.Model;
        dbProduct.Type = vehicle.Type;
        dbProduct.Year = vehicle.Year;

        await _context.SaveChangesAsync();

        return Ok(vehicle);
    }
    
    [HttpGet("/Get")]
    public async Task<ActionResult> GetVehicles()
    {
        var vehicles = await _context.Vehicle
            .Select(v => new 
            {
                v.Id,
                v.User,
                v.Model,
                v.Type,
                v.Year,
                v.Value
            })
            .ToListAsync();

        return Ok(vehicles);
    }
}