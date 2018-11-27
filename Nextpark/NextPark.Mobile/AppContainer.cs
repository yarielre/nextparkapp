using System;
using Autofac;
using NextPark.Mobile.ViewModels;

namespace NextPark.Mobile.Infrastructure
{
    public class AppContainer
    {
        private static IContainer _container;

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            //Register ViewModels
            builder.RegisterType<StartUpViewModel>();

            //Services - Data

            //Services - Application


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
