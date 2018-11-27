using NextPark.Mobile.ViewModels;

namespace NextPark.Mobile.Infrastructure
{
    public class ViewModelLocator
    {
             public StartUpViewModel StartUp => AppContainer.Resolve<StartUpViewModel>();
    }
}
