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

            //CreateMap<Event, EventModel>()
            // //   .ForMember(evm => evm.MonthRepeat, cfg => cfg.ResolveUsing<MonthOfYearsResolver>())
            //    .ForMember(evm => evm.WeekRepeat, cfg => cfg.ResolveUsing<DayOfWeeksResolver>());
            //CreateMap<EventModel, Event>()
            // //   .ForMember(e => e.MonthRepeat, cfg => cfg.ResolveUsing<MotnthOfYearFromViewModelResolver>())
            //    .ForMember(e => e.WeekRepeat, cfg => cfg.ResolveUsing<DayOfWeeksFromViewModelResolver>());
            CreateMap<ApplicationUser, UserModel>()
                .ForMember(pvm => pvm.ImageUrl, cfg => cfg.MapFrom(p => p.ImageUrl))
                .ReverseMap();
            CreateMap<UserModel, ApplicationUser>()
                .ForMember(pvm => pvm.ImageUrl, cfg => cfg.MapFrom(p => p.ImageUrl))
                .ReverseMap();
            CreateMap<ParkingModel, Parking>()
                 .ForMember(pvm => pvm.ImageUrl, cfg => cfg.MapFrom(p => p.ImageUrl));
            CreateMap<Parking, ParkingModel>()
                .ForMember(pvm => pvm.ImageUrl, cfg => cfg.MapFrom(p => p.ImageUrl));
            CreateMap<Order, OrderModel>()
                .ReverseMap();
        }
    }
}
