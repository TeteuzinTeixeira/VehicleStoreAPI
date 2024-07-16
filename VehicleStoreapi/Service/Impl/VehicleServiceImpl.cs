using Microsoft.EntityFrameworkCore;
using VehicleStoreapi.Database;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities.Dto;

namespace VehicleStoreapi.Service.Impl;

public class VehicleServiceImpl : IVehicleService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public VehicleServiceImpl(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }
    public bool ValidarVehicle(Vehicle vehicle)
    {
        if (vehicle.Equals(null))
        {
            return false;
        }
        
        return !(vehicle.Value <= (decimal)0.0);
    }

    public async Task<Vehicle> SaveVehicle(Vehicle vehicle, List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
        {
            throw new ArgumentException("Nenhuma imagem foi enviada.");
        }
        
        _context.Vehicle.Add(vehicle);
        await _context.SaveChangesAsync();

        var vehicleImages = await SaveImages(vehicle, files);

        _context.VehicleImage.AddRange(vehicleImages);
        await _context.SaveChangesAsync();

        return vehicle;
    }

    public async Task<List<VehicleImage>> SaveImages(Vehicle vehicle, List<IFormFile> files)
    {
        var vehicleImages = new List<VehicleImage>();
        const string imagePath = @"C:\Mateus\VehicleStoreAPI\Imagens";

        if (!Directory.Exists(imagePath))
        {
            Directory.CreateDirectory(imagePath);
        }

        foreach (var file in files)
        {
            var fileId = Guid.NewGuid();
            var fileName = $"{fileId}.jpg";
            var path = Path.Combine(imagePath, fileName);

            await using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var vehicleImage = new VehicleImage
            {
                Id = fileId,
                VehicleId = vehicle.Id,
                Path = path
            };

            vehicleImages.Add(vehicleImage);
        }

        return vehicleImages;
    }
    
    public async Task<bool> UpdateVehicleAsync(Guid id, Vehicle vehicle)
    {
        var dbVehicle = await _context.Vehicle.FirstOrDefaultAsync(v => v.Id == id);
        if (dbVehicle == null)
        {
            return false;
        }

        dbVehicle.Value = vehicle.Value;
        dbVehicle.Model = vehicle.Model;
        dbVehicle.Type = vehicle.Type;
        dbVehicle.Year = vehicle.Year;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!VehicleExists(id))
            {
                return false;
            }

            throw;
        }
    }

    public bool VehicleExists(Guid id)
    {
        return _context.Vehicle.Any(e => e.Id == id);
    }
    
    public async Task<bool> UpdateVehicleImagesAsync(Guid vehicleId, List<Guid> removedImageIds, List<IFormFile> files)
    {
        var vehicleExists = await _context.Vehicle.AnyAsync(v => v.Id == vehicleId);
        if (!vehicleExists)
        {
            return false;
        }
        
        var imagesToRemove = await _context.VehicleImage
            .Where(i => removedImageIds.Contains(i.Id) && i.VehicleId == vehicleId)
            .ToListAsync();
        _context.VehicleImage.RemoveRange(imagesToRemove);
        
        foreach (var file in files)
        {
            if (file.Length <= 0) continue;
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", file.FileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
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

        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<VehicleDto?> GetVehicleByIdAsync(Guid id)
    {
        var vehicle = await _context.Vehicle
            .Where(v => v.Id == id)
            .Select(v => new VehicleDto
            {
                Id = v.Id,
                Model = v.Model,
                Type = v.Type,
                Year = v.Year,
                Value = v.Value
            })
            .FirstOrDefaultAsync();

        return vehicle;
    }

    public async Task<List<VehicleImageDto>> GetImagesByVehicleIdAsync(Guid id)
    {
        var images = await _context.VehicleImage
            .Where(vi => vi.VehicleId == id)
            .Select(vi => new VehicleImageDto
            {
                Id = vi.Id,
                VehicleId = vi.VehicleId,
                Path = vi.Path
            })
            .ToListAsync();

        return images;
    }

    public async Task<List<VehicleWithImagesDto>> GetVehiclesAndImagesAsync()
    {
        var vehicles = await _context.Vehicle
            .GroupJoin(
                _context.VehicleImage,
                v => v.Id,
                vi => vi.VehicleId,
                (v, vehicleImages) => new { Vehicle = v, VehicleImages = vehicleImages }
            )
            .Select(g => new VehicleWithImagesDto
            {
                Id = g.Vehicle.Id,
                Model = g.Vehicle.Model,
                Type = g.Vehicle.Type,
                Year = g.Vehicle.Year,
                Value = g.Vehicle.Value,
                VehicleImages = g.VehicleImages.Select(vi => new VehicleImageDto
                {
                    Id = vi.Id,
                    Path = vi.Path
                }).ToList()
            })
            .ToListAsync();

        return vehicles;
    }
}