using System;
using Autofac;
using NextPark.Mobile.Services;
using NextPark.Mobile.Services.Data;
using NextPark.Mobile.ViewModels;

namespace NextPark.Mobile.Infrastructure
{
    public class IoCSettings
    {
        private static IContainer _container;

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            //Services - Application
            builder.RegisterType<ApiService>().As<IApiService>();
            builder.RegisterType<LocalizationService>().As<ILocalizationService>();
            builder.RegisterType<AuthService>().As<IAuthService>().SingleInstance();
            builder.RegisterType<NavigationService>().As<INavigationService>();
            builder.RegisterType<DialogService>().As<IDialogService>();
            builder.RegisterType<GeolocatorService>().As<IGeolocatorService>();
            builder.RegisterType<LoggerService>().As<ILoggerService>();
<<<<<<< HEAD
            builder.RegisterType<ProfileService>().As<IProfileService>().SingleInstance();
=======
            builder.RegisterType<InAppPurchaseService>().As<InAppPurchaseService>();
>>>>>>> master

            //Services - Data
            builder.RegisterType<ParkingDataService>();
            builder.RegisterType<EventDataService>();
            builder.RegisterType<OrderDataService>();

            //Register ViewModels
            builder.RegisterType<LocatorViewModel>();
            builder.RegisterType<StartUpViewModel>();
            builder.RegisterType<AddEventViewModel>();
            builder.RegisterType<AddParkingViewModel>();
            builder.RegisterType<BookingMapViewModel>();
            builder.RegisterType<BookingViewModel>();
            builder.RegisterType<HomeViewModel>();
            builder.RegisterType<LaunchScreenViewModel>();
            builder.RegisterType<LoginViewModel>();
            builder.RegisterType<MoneyViewModel>();
            builder.RegisterType<ParkingDataViewModel>();
            builder.RegisterType<RegisterViewModel>();
            builder.RegisterType<UserBookingViewModel>();
            builder.RegisterType<UserDataViewModel>();
            builder.RegisterType<UserParkingViewModel>();
            builder.RegisterType<UserProfileViewModel>();

            _container = builder.Build();
        }

        public static object Resolve(Type typeName)
        {
            return _container.Resolve(typeName);
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
