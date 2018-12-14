using NextPark.Mobile.Services;
using NextPark.Mobile.ViewModels;
using System;

namespace NextPark.Mobile.Infrastructure
{
    public class LocatorViewModel
    {
        public StartUpViewModel StartUp => IoCSettings.Resolve<StartUpViewModel>();

        public HomeViewModel Home
        {
            get
            {
                HomeViewModel vm = null;

                try
                {
                    vm = IoCSettings.Resolve<HomeViewModel>();
                }
                catch (Exception e)
                {

                }
                return vm;
            }
        }
    }
}
