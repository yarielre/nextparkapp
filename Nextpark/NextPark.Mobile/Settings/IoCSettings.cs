using System;
using Autofac;
using NextPark.Mobile.Services;
using NextPark.Mobile.ViewModels;

namespace NextPark.Mobile.Infrastructure
{
    public class IoCSettings
    {
        private static IContainer _container;

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            //Register ViewModels
            builder.RegisterType<StartUpViewModel>();
            builder.RegisterType<HomeViewModel>();

            //Services - Data
            builder.RegisterType<ParkingDataService>().As<IParkingDataService>();

            //Services - Application
            builder.RegisterType<ApiService>().As<IApiService>();
            builder.RegisterType<AuthService>().As<IAuthService>().SingleInstance();
            builder.RegisterType<NavigationService>().As<INavigationService>();
            builder.RegisterType<DialogService>().As<IDialogService>();


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
