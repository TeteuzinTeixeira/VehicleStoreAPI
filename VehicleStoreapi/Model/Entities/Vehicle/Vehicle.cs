using Microsoft.AspNetCore.Identity;
using System;

namespace VehicleStoreapi.Database.Vehicle
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Year { get; set; }
        public double Value { get; set; }
        public ICollection<VehicleImage>? Images { get; set; }
    }
}