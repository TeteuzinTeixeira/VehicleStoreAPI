using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleStoreapi.Database;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities.Dto;
using VehicleStoreapi.Service;

namespace VehicleStoreapi;

[Authorize]
[Route("[controller]")]
[ApiController]
public class VehicleController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly VehicleService _service;
    private readonly ILogger<VehicleController> _logger;
    private readonly IWebHostEnvironment _environment;

    public VehicleController(AppDbContext context, VehicleService service, ILogger<VehicleController> logger, IWebHostEnvironment environment)
    {
        _context = context;
        _service = service;
        _logger = logger;
        _environment = environment;
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
    [HttpPut("UpdateVehicle/{id}")]
    public async Task<ActionResult> UpdateVehicle(Guid id, [FromForm] Vehicle vehicle)
    {
        if (id != vehicle.Id)
        {
            return BadRequest($"Vehicle não encontrado para o Id: {id}");
        }

        var dbProduct = await _context.Vehicle
            .FirstOrDefaultAsync(v => v.Id == id);

        if (dbProduct == null)
        {
            return NotFound();
        }

        dbProduct.Value = vehicle.Value;
        dbProduct.Model = vehicle.Model;
        dbProduct.Type = vehicle.Type;
        dbProduct.Year = vehicle.Year;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(vehicle);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!VehicleExists(id))
            {
                return NotFound($"Vehicle não encontrado com o ID: {id}");
            }

            throw;
        }
    }

    private bool VehicleExists(Guid id)
    {
        return _context.Vehicle.Any(e => e.Id == id);
    }
    
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("UpdateVehicleImages/{vehicleId}")]
    public async Task<IActionResult> UpdateVehicleImages(Guid vehicleId, [FromForm] List<Guid> removedImageIds, [FromForm] List<IFormFile> files)
    {
        var vehicleExists = await _context.Vehicle.AnyAsync(v => v.Id == vehicleId);
        if (!vehicleExists)
        {
            return NotFound();
        }
        
        var imagesToRemove = await _context.VehicleImage
            .Where(i => removedImageIds.Contains(i.Id) && i.VehicleId == vehicleId)
            .ToListAsync();
        _context.VehicleImage.RemoveRange(imagesToRemove);
        
        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", file.FileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                var newImage = new VehicleImage
                {
                    Path = $"/uploads/{file.FileName}",
                    VehicleId = vehicleId
                };
                _context.VehicleImage.Add(newImage);
            }
        }

        await _context.SaveChangesAsync();

        return Ok();
    }
    
    [HttpGet("GetImagesByVehicleId/{id}")]
    public async Task<IActionResult> GetImagesByVehicleId(Guid id)
    {
        var images = await _context.VehicleImage
            .Where(vi => vi.VehicleId == id)
            .Select(vi => new
            {
                vi.Id,
                vi.VehicleId,
                vi.Path
            })
            .ToListAsync();

        if (!images.Any())
        {
            return NotFound();
        }

        return Ok(images);
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