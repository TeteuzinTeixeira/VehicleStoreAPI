using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleStoreapi.Database.Vehicle;
public class VehicleImage
    {
        public Guid Id { get; set; }
        
        [ForeignKey("Vehicle")]
        public Guid VehicleId { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }
        public string Path { get; set; }
    }