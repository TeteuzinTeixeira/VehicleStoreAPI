using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleStoreapi.Model.Entities.Dto;

public class VehicleDto
{
    public Guid Id { get; set; }
    public string Model { get; set; }
    public string Type { get; set; }
    public string Year { get; set; }
    public decimal Value { get; set; }
    [NotMapped]
    public ICollection<VehicleImageDto> VehicleImages { get; set; }
}