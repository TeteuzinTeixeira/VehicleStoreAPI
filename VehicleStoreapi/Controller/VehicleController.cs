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
    public async Task<ActionResult> PostVehicle([FromForm] Vehicle vehicle, FileModel fileModel)
    {
        try
        {
            // Gera um UUID para o arquivo
            var fileId = Guid.NewGuid();
        
            // Define o caminho completo usando o UUID
            string fileName = $"{fileId}.jpg"; // Altere a extensão conforme necessário
            string path = Path.Combine(@"C:\Mateus\VehicleStoreAPI\Imagens", fileName);

            // Salva o arquivo no caminho especificado
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await fileModel.File.CopyToAsync(stream);
            }

            // Cria a entidade VehicleImage e define seus valores
            var vehicleImage = new VehicleImage
            {
                Id = fileId,
                VehicleId = vehicle.Id,
                UserId = vehicle.UserId,
                Path = path
            };

            // Adiciona a entidade VehicleImage ao contexto
            _context.VehicleImage.Add(vehicleImage);
        }
        catch (Exception e)
        {
            return BadRequest($"Failed to save file: {e.Message}");
        }

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