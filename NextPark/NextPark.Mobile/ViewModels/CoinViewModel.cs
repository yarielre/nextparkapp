using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NextPark.Mobile.Services;
using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
    public class CoinViewModel : BaseViewModel
    {
        public CoinViewModel(IApiService apiService, IAuthService authService, INavigationService navService) : base(apiService, authService, navService)
        {
        }

        public override Task InitializeAsync(object data = null)
        {
            return base.InitializeAsync(data);
        }

        public ICommand GoToHome => new Command(GoToHomeMethod);

        public void GoToHomeMethod()
        {
            NavigationService.NavigateToAsync<HomeViewModel>();
        }

    }
}
