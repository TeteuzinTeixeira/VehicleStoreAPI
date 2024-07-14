using VehicleStoreapi.Database;
using VehicleStoreapi.Database.Vehicle;

namespace VehicleStoreapi.Service.Impl;

public class VehicleServiceImpl : VehicleService
{
    private readonly AppDbContext _context;

    public VehicleServiceImpl(AppDbContext context)
    {
        _context = context;
    }
    public bool ValidarVehicle(Vehicle vehicle)
    {
        if (vehicle.Equals(null))
        {
            return false;
        }
        
        if (vehicle.Value <= 0.0)
        {
            return false;
        }

        return true;
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
                Path = path
            };

            vehicleImages.Add(vehicleImage);
        }

        return vehicleImages;
    }
}