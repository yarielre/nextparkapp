using System;
using System.Collections.Generic;
using System.Text;
using NextPark.Mobile.Services;

namespace NextPark.Mobile.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel(INavigationService navService, IApiService apiService, IAuthService authService) : base(navService, apiService, authService)
        {
        }
    }
}
