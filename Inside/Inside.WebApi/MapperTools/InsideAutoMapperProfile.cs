using System.IO;
using AutoMapper;
using Inside.Domain.Entities;
using Inside.WebApi.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;

namespace Inside.WebApi.MapperTools
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

            CreateMap<Event, EventViewModel>()
                .ForMember(evm => evm.MonthRepeat, cfg => cfg.ResolveUsing<MonthOfYearsResolver>())
                .ForMember(evm => evm.WeekRepeat, cfg => cfg.ResolveUsing<DayOfWeeksResolver>());
            CreateMap<EventViewModel, Event>()
                .ForMember(e => e.MonthRepeat, cfg => cfg.ResolveUsing<MotnthOfYearFromViewModelResolver>())
                .ForMember(e => e.WeekRepeat, cfg => cfg.ResolveUsing<DayOfWeeksFromViewModelResolver>());

            CreateMap<ParkingViewModel, Parking>();
            CreateMap<Parking, ParkingViewModel>()
                .ForMember(pvm => pvm.Status, cfg => cfg.MapFrom(p => p.IsRented ? "Rented" : "Avialable"))
                .ForMember(pvm => pvm.RentInfo,
                    cfg => cfg.MapFrom(p => p.Orders.Find(o => o.OrderStatus == Domain.Enum.OrderStatus.Actived)))

                //In case the app will be host online chage localhost:5041 for app hotname or ip
                .ForMember(pvm => pvm.ImageUrl,
                    cfg => cfg.MapFrom(p =>
                        Path.Combine(hostname + "/Images/", p.ImageUrl)))
                .ForMember(pvm => pvm.ParkingType, cfg => cfg.MapFrom(p => p.ParkingType));

            CreateMap<ParkingType, ParkingTypeViewModel>();
            CreateMap<ParkingCategory, ParkingCategoryViewModel>();
        }
    }
}