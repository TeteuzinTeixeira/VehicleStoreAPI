using AutoMapper;
using VehicleStoreapi.Database.Vehicle;
using VehicleStoreapi.Model.Entities.Dto;

namespace VehicleStoreapi.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Order, OrderDto>()
            .ReverseMap();
    }
}