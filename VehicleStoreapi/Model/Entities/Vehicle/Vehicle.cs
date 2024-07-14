using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleStoreapi.Database.Vehicle
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Year { get; set; }
        public double Value { get; set; }
    }
}