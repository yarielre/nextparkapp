using System;
using System.Threading.Tasks;

namespace NextPark.Mobile.Core.Services
{
    public interface INavigationService
    {
        Task ClearBackStack();
        Task InitializeAsync();
        Task NavigateBackAsync();
        Task NavigateToAsync(Type viewModelType);
        Task NavigateToAsync(Type viewModelType, object parameter);
        Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase;
        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase;
        Task PopToRootAsync();
        Task RemoveLastFromBackStackAsync();
    }
}