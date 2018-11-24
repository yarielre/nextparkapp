using GalaSoft.MvvmLight.Command;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Services;
using NextPark.Models;
using System.Windows.Input;
using Xamarin.Forms;
namespace Inside.Xamarin.ViewModels
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        private string _newPassword;
        private string _oldPassword;

        public string NewPassword
        {
            get => _newPassword;
            set => SetValue(ref _newPassword, value);
        }
        public string OldPassword
        {
            get => _oldPassword;
            set => SetValue(ref _oldPassword, value);
        }
        public bool IsRunning { get; set; }
        public bool IsEnabled { get; set; }

        public ICommand ChangeCommand => new RelayCommand(Change);

        private async void Change()
        {
            if (string.IsNullOrEmpty(OldPassword))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.GeneralPasswordError,
                    "Your old password should not be empty",
                   Languages.GeneralAccept);
                return;
            }
            if (string.IsNullOrEmpty(NewPassword))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.GeneralPasswordError,
                    "Your new password should not be empty",
                   Languages.GeneralAccept);
                return;
            }
            IsRunning = true;
            IsEnabled = false;
            var userResponse = await InsideApi.GetUserByUserName(HostSetting.AuthEndPoint, MainViewModel.GetInstance().CurrentUser.Name);

            var model = new ChangePasswordViewModel
            {
                Id = ((UserModel)userResponse.Result).Id,
                OldPassword = OldPassword,
                NewPassword = NewPassword,
            };

            var response = await InsideApi.Post<ChangePasswordViewModel, TokenResponse>(
                HostSetting.ChangePasswordEndPoint,
                model);

            if (response == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.GeneralError,
                    Languages.LoginErrorServerAlert +
                    "\n" +
                   Languages.GeneralCheckInternetConnection +
                    "\n",
                   Languages.GeneralAccept, Languages.GeneralCancel);
            }
            else
            {
                if (response.IsSuccess)
                {
                    await Application.Current.MainPage.DisplayAlert(
                       Languages.GeneralSuccess,
                       Languages.ChangePasswordSuccessAlert,
                       Languages.GeneralAccept);
                    ResetLoginData();
                    NavigationService.GetInstance().SetMainPage(Pages.LoginView);
                }
                else
                    await Application.Current.MainPage.DisplayAlert(
                        Languages.GeneralError,
                        Languages.EditProfileOldPasswordAlert,
                        Languages.GeneralAccept);
            }
            IsRunning = false;
            IsEnabled = true;
        }

        public void ResetLoginData()
        {
            InsideApi.AuthToken = string.Empty;
            //CurrentUser = null;
            Settings.UserId = string.Empty;
            Settings.UserName = string.Empty;
            Settings.Token = string.Empty;
        }
    }
}
