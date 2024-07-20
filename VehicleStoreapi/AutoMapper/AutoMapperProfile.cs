using AutoMapper;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities.Dto;

namespace VehicleStoreapi.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Vehicle, VehicleDto>();
        CreateMap<VehicleDto, Vehicle>();
        CreateMap<Order, OrderDto>();
        CreateMap<OrderDto, Order>();
        CreateMap<VehicleWithImagesDto, VehicleDto>();
    }
}