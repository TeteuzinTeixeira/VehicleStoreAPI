using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleStoreapi.Database;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model;
using VehicleStoreapi.Model.Entities;
using VehicleStoreapi.Service;

namespace VehicleStoreapi;

[Route("vehicle/[controller]")]
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
    
    [HttpPost("/SaveVehicle")]
    public async Task<ActionResult> PostVehicle([FromForm] Vehicle vehicle, [FromForm] List<IFormFile> files)
    {
        try
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("Nenhuma imagem foi enviada.");
            }
            
            _context.Vehicle.Add(vehicle);
            await _context.SaveChangesAsync();

            var vehicleImages = new List<VehicleImage>();
            string imagePath = @"C:\Mateus\VehicleStoreAPI\Imagens";
            
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            foreach (var file in files)
            {
                var fileId = Guid.NewGuid();
                string fileName = $"{fileId}.jpg";
                string path = Path.Combine(imagePath, fileName);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var vehicleImage = new VehicleImage
                {
                    Id = fileId,
                    VehicleId = vehicle.Id,
                    UserId = vehicle.UserId,
                    Path = path
                };

                vehicleImages.Add(vehicleImage);
            }
            
            _context.VehicleImage.AddRange(vehicleImages);
            await _context.SaveChangesAsync();

            return Ok(vehicle);
        }
        catch (Exception e)
        {
            return BadRequest($"Failed to save file: {e.Message}");
        }
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

        dbProduct.Id = vehicle.Id;
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
                v.Model,
                v.Type,
                v.Year,
                v.Value
            })
            .ToListAsync();

        return Ok(vehicles);
    }
}