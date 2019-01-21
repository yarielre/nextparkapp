using AutoMapper;
using NextPark.Domain.Entities;
using NextPark.Enums;
using NextPark.Models;

namespace NextPark.MapperTools
{
    public class InsideAutoMapperProfile : Profile
    {
        public InsideAutoMapperProfile()
            : this("MyProfile")
        {
            
        }

        protected InsideAutoMapperProfile(string profileName)
            : base(profileName)
        {

            CreateMap<Event, EventModel>()
             //   .ForMember(evm => evm.MonthRepeat, cfg => cfg.ResolveUsing<MonthOfYearsResolver>())
                .ForMember(evm => evm.WeekRepeat, cfg => cfg.ResolveUsing<DayOfWeeksResolver>());
            CreateMap<EventModel, Event>()
             //   .ForMember(e => e.MonthRepeat, cfg => cfg.ResolveUsing<MotnthOfYearFromViewModelResolver>())
                .ForMember(e => e.WeekRepeat, cfg => cfg.ResolveUsing<DayOfWeeksFromViewModelResolver>());
            CreateMap<ApplicationUser, UserModel>()
                .ReverseMap();
            CreateMap<ApplicationUser, UserModel>()
                .ReverseMap();
            CreateMap<ParkingModel, Parking>()
                 .ForMember(pvm => pvm.ImageUrl, cfg => cfg.MapFrom(p => p.ImageUrl));
            CreateMap<Parking, ParkingModel>()
                .ForMember(pvm => pvm.ImageUrl, cfg => cfg.MapFrom(p => p.ImageUrl));

            CreateMap<ParkingCategory, ParkingCategoryModel>()
                .ReverseMap();
            CreateMap<ParkingType, ParkingTypeModel>()
                .ReverseMap();
            CreateMap<Order, OrderModel>()
                .ReverseMap();
        }
    }
}
