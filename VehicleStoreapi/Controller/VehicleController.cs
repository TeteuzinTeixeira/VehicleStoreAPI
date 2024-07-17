using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleStoreapi.Database;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities.Dto;
using VehicleStoreapi.Service;

namespace VehicleStoreapi.Controller;

[Authorize]
[Route("[controller]")]
[ApiController]
public class VehicleController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IVehicleService _service;
    private readonly IMapper _mapper;

    public VehicleController(AppDbContext context, IVehicleService service, IMapper mapper)
    {
        _context = context;
        _service = service;
        _mapper = mapper;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("Save")]
    public async Task<ActionResult<VehicleDto>> PostVehicle([FromForm] Vehicle vehicle, [FromForm] List<IFormFile> files)
    {
        try
        {
            var savedVehicle = await _service.SaveVehicle(vehicle, files);
            var vehicleDto = _mapper.Map<VehicleDto>(savedVehicle);
            return Ok(vehicleDto);
        }
        catch (Exception e)
        {
            return BadRequest($"Failed to save vehicle: {e.Message}");
        }
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpDelete("Delete/{id:guid}")]
    public async Task<ActionResult> DeleteVehicle(Guid id)
    {
        var dbProduct = await _context.Vehicle.FindAsync(id);

        if (dbProduct == null)
        {
            return NotFound();
        }

        _context.Vehicle.Remove(dbProduct);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPut("UpdateVehicle/{id:guid}")]
    public async Task<ActionResult<VehicleDto>> UpdateVehicle(Guid id, [FromForm] Vehicle vehicle)
    {
        if (id != vehicle.Id)
        {
            return BadRequest($"Vehicle não encontrado para o Id: {id}");
        }

        var existingVehicle = await _service.GetVehicleByIdAsync(id);
        if (existingVehicle == null)
        {
            return NotFound($"Vehicle não encontrado para o Id: {id}");
        }

        var result = await _service.UpdateVehicleAsync(id, vehicle);

        if (!result)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Falha ao atualizar o veículo");
        }

        var vehicleDto = _mapper.Map<VehicleDto>(vehicle);

        return Ok(vehicleDto);
    }
    
    [HttpPost("UpdateVehicleImages/{vehicleId:guid}")]
    public async Task<IActionResult> UpdateVehicleImages(Guid vehicleId, [FromForm] List<Guid> removedImageIds, [FromForm] List<IFormFile> files)
    {
        var result = await _service.UpdateVehicleImagesAsync(vehicleId, removedImageIds, files);

        if (!result)
        {
            return NotFound();
        }

        return Ok();
    }
    
    [HttpGet("GetImagesByVehicleId/{id:guid}")]
    public async Task<IActionResult> GetImagesByVehicleId(Guid id)
    {
        var images = await _service.GetImagesByVehicleIdAsync(id);

        if (images.Count == 0)
        {
            return NotFound();
        }

        return Ok(images);
    }

    [HttpGet("GetVehicleById/{id:guid}")]
    public async Task<IActionResult> GetVehicleById(Guid id)
    {
        var vehicle = await _service.GetVehicleByIdAsync(id);

        if (vehicle == null)
        {
            return NotFound($"Vehicle não encontrado com o id: {id}");
        }
        
        var vehicleDto = _mapper.Map<VehicleDto>(vehicle);

        return Ok(vehicleDto);
    }

    [HttpGet("GetVehiclesAndImages")]
    public async Task<IActionResult> GetVehiclesAndImages()
    {
        var vehicles = await _service.GetVehiclesAndImagesAsync();

        if (vehicles.Count == 0)
        {
            return NotFound();
        }
        
        var vehicleDto = _mapper.Map<List<VehicleDto>>(vehicles);

        return Ok(vehicleDto);
    }
}