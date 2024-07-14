using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleStoreapi.Database;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities;
using VehicleStoreapi.Service;

namespace VehicleStoreapi;

[Route("[controller]")]
[ApiController]
public class VehicleController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly VehicleService _service;
    private readonly ILogger<VehicleController> _logger;

    public VehicleController(AppDbContext context, VehicleService service, ILogger<VehicleController> logger)
    {
        _context = context;
        _service = service;
        _logger = logger;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("Save")]
    public async Task<ActionResult> PostVehicle([FromForm] Vehicle vehicle, [FromForm] List<IFormFile> files)
    {
        try
        {
            var savedVehicle = await _service.SaveVehicle(vehicle, files);
            _logger.LogInformation("Saved Vehicle: {vehicle}", vehicle);
            return Ok(savedVehicle);
        }
        catch (Exception e)
        {
            _logger.LogInformation("Failed to save vehicle");
            return BadRequest($"Failed to save vehicle: {e.Message}");
        }
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult> DeleteVehicle(Guid id)
    {
        _logger.LogInformation("DeleteVehicle method called with id: {id}", id);
        var dbProduct = await _context.Vehicle.FindAsync(id);

        if (dbProduct == null)
        {
            _logger.LogWarning("Vehicle not found with id: {id}", id);
            return NotFound();
        }

        _context.Vehicle.Remove(dbProduct);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Vehicle deleted with id: {id}", id);
        return NoContent();
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPut("Update/{id}")]
    public async Task<ActionResult> UpdateProduct(Guid id, [FromForm] Vehicle vehicle, [FromForm] List<IFormFile> files)
    {
        if (id != vehicle.Id)
        {
            return BadRequest("O ID do veículo na URL não corresponde ao ID do veículo na requisição.");
        }

        var dbProduct = await _context.Vehicle
            .Include(v => v.VehicleImages)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (dbProduct == null)
        {
            return NotFound();
        }
        
        dbProduct.Value = vehicle.Value;
        dbProduct.Model = vehicle.Model;
        dbProduct.Type = vehicle.Type;
        dbProduct.Year = vehicle.Year;
        
        if (files != null && files.Count > 0)
        {
            var updatedImages = await _service.SaveImages(vehicle, files);
            
            _context.VehicleImage.RemoveRange(dbProduct.VehicleImages);
            
            dbProduct.VehicleImages = updatedImages;
        }

        try
        {
            await _context.SaveChangesAsync();
            return Ok(vehicle);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!VehicleExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    private bool VehicleExists(Guid id)
    {
        return _context.Vehicle.Any(e => e.Id == id);
    }

    
    [HttpGet("Get")]
    public async Task<ActionResult> GetVehicles()
    {
        var vehicles = await _context.Vehicle
            .Join(_context.VehicleImage,
                v => v.Id,
                vi => vi.VehicleId,
                (v, vi) => new
                {
                    v.Id,
                    v.Model,
                    v.Type,
                    v.Year,
                    v.Value,
                    VehicleImages = new { vi.Id, vi.Path }
                })
            .ToListAsync();

        return Ok(vehicles);
    }
}