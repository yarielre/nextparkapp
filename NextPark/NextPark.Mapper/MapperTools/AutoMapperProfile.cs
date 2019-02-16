using AutoMapper;
using NextPark.Domain.Entities;
using NextPark.Enums;
using NextPark.Models;

namespace NextPark.MapperTools
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
            : this("MyProfile")
        {

        }

        protected AutoMapperProfile(string profileName)
            : base(profileName)
        {

            CreateMap<Event, EventModel>()
                .ForMember(evm => evm.ParkingId, cfg => cfg.MapFrom(p => p.ParkingId))
                  .ReverseMap();
            CreateMap<EventModel, Event>()
                .ForMember(evm => evm.ParkingId, cfg => cfg.MapFrom(p => p.ParkingId))
                  .ReverseMap();
            CreateMap<ApplicationUser, UserModel>()
                .ForMember(pvm => pvm.ImageUrl, cfg => cfg.MapFrom(p => p.ImageUrl))
                .ReverseMap();
            CreateMap<UserModel, ApplicationUser>()
                .ForMember(pvm => pvm.ImageUrl, cfg => cfg.MapFrom(p => p.ImageUrl))
                .ReverseMap();
            CreateMap<ParkingModel, Parking>()
                 .ForMember(pvm => pvm.ImageUrl, cfg => cfg.MapFrom(p => p.ImageUrl))
                   .ReverseMap();
            CreateMap<Parking, ParkingModel>()
                .ForMember(pvm => pvm.ImageUrl, cfg => cfg.MapFrom(p => p.ImageUrl));
            CreateMap<Order, OrderModel>()
                .ReverseMap();
        }
    }
}
