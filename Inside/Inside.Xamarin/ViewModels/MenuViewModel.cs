using GalaSoft.MvvmLight.Command;
using Inside.Xamarin.Services;
using System.Windows.Input;

namespace Inside.Xamarin.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        private string _username;
        public string UserName
        {
            get => _username;
            set => SetValue(ref _username, value);
        }

        private string _coins;
        public string Coins
        {
            get => _coins;
            set => SetValue(ref _coins, value);
        }
        public ICommand OnCoinActionCommand => new RelayCommand(GoToCoin);

        private void GoToCoin()
        {
            NavigationService.GetInstance().FocusCoinsTab();
        }
    }
}
