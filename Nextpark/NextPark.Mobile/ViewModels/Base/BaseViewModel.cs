using System;
using NextPark.Mobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NextPark.Mobile.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private readonly INavigationService _navService;
        public INavigationService NavigationService => _navService;

        private readonly IApiService _apiService;
        public IApiService ApiService => _apiService;

        private readonly IAuthService _authService;
        public IAuthService AuthService => _authService;

        public int Id { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public BaseViewModel(INavigationService navService, IApiService apiService, IAuthService authService)
        {
            _apiService = apiService;
            _authService = authService;
            _navService = navService;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return;
            }
            backingField = value;
            OnPropertyChanged(propertyName);
        }

        public virtual Task InitializeAsync(object data = null)
        {
            return Task.FromResult(false);
        }

        public virtual Task CallBackAsync(object data)
        {
            return Task.FromResult(false);
        }


        protected virtual void CurrentPageOnAppearing(object sender, System.EventArgs eventArgs) { }

        protected virtual void CurrentPageOnDisappearing(object sender, System.EventArgs eventArgs) { }

    }
}
