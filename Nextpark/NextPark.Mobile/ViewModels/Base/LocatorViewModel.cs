using NextPark.Mobile.ViewModels;

namespace NextPark.Mobile.Infrastructure
{
    public class LocatorViewModel
    {
             public StartUpViewModel StartUp => IoCSettings.Resolve<StartUpViewModel>();
    }
}
