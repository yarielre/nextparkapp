using System;
using AutoMapper;
using NextPark.Domain.Entities;
using NextPark.Enums;
using NextPark.Enums.Enums;
using NextPark.Models;
using NextPark.Models.Models;

namespace NextPark.MapperTools
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
            : this("MyProfile")
        {
            CreateMap<Transaction, TransactionModel>()
                .ForMember(vm => vm.User, cfg => cfg.MapFrom(t => t.User.Email))
                .ForMember(vm => vm.TransactionStatus,
                    cfg => cfg.MapFrom(t =>
                        Enum.GetName(typeof(TransactionStatus), t.Status)))
                .ForMember(vm => vm.TransactionType,
                    cfg => cfg.MapFrom(t =>
                        Enum.GetName(typeof(TransactionType), t.Type)))
                .ForMember(vm => vm.CreationTime, cfg => cfg.MapFrom(t => t.CreationDate.ToString("dd/MM/yyyy hh:mm")))
                .ForMember(vm => vm.CompleteTime,
                    cfg => cfg.MapFrom(t => t.CompletationDate.ToString("dd/MM/yyyy hh:mm")));
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
