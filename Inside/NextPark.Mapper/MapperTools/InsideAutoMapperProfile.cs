using AutoMapper;
using NextPark.Domain.Entities;
using NextPark.Enums;
using NextPark.Models;

namespace NextPark.MapperTools
{
    public class InsideAutoMapperProfile : Profile
    {
        public InsideAutoMapperProfile(string hostname)
            : this("MyProfile", hostname)
        {
            
        }

        protected InsideAutoMapperProfile(string profileName, string hostname)
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
                .ForMember(pvm => pvm.Status, cfg => cfg.MapFrom(p => p.IsRented ? "Rented" : "Avialable"))
                .ForMember(pvm => pvm.RentByHour, cfg => cfg.MapFrom(p => p.Orders.Find(o=>o.OrderStatus==OrderStatus.Actived && o.Parking.ParkingType.Type=="By Hours")))
                .ForMember(pvm => pvm.RentForMonth, cfg => cfg.MapFrom(p => p.Orders.Find(o=>o.OrderStatus==OrderStatus.Actived && o.Parking.ParkingType.Type=="For Month"))) 
                .ForMember(pvm => pvm.ParkingType, cfg => cfg.MapFrom(p => p.ParkingType))
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
