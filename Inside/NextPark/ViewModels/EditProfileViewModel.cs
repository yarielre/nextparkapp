using GalaSoft.MvvmLight.Command;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Services;
using NextPark.Models;
using System.Windows.Input;
using Xamarin.Forms;

namespace Inside.Xamarin.ViewModels
{
    public class EditProfileViewModel : BaseViewModel
    {
        private string _newPassword;
        private string _oldPassword;

        public string UserName { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string OldPassword
        {
            get => _oldPassword;
            set => SetValue(ref _oldPassword, value);
        }
        public string NewPassword
        {
            get => _newPassword;
            set => SetValue(ref _newPassword, value);
        }
        public string Address { get; set; }
        public string State { get; set; }
        public string CarPlate { get; set; }

        public bool IsRunning { get; set; }
        public bool IsEnabled { get; set; }

        public ICommand EditCommand => new RelayCommand(Edit);

        public EditProfileViewModel()
        {
            UserModel user = MainViewModel.GetInstance().CurrentUser;
            UserName = user.UserName;
            Name = user.Name;
            Lastname = user.Lastname;
            Email = user.Email;
            Address = user.Address;
            State = user.State;
            CarPlate = user.CarPlate;
        }

        private async void Edit()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.GeneralError,
                    Languages.RegisterUserEmptyAlert,
                   Languages.GeneralAccept);
                return;
            }
            if (string.IsNullOrEmpty(Lastname))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.GeneralError,
                    "Your lastname should not be empty",
                   Languages.GeneralAccept);
                return;
            }
            if (string.IsNullOrEmpty(Email))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.GeneralError,
                     Languages.RegisterEmailEmptyAlert,
                   Languages.GeneralAccept);
                return;
            }
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
            if (string.IsNullOrEmpty(Address))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Your address should not be empty",
                   Languages.GeneralAccept);
                return;
            }
            if (string.IsNullOrEmpty(State))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Your state should not be empty",
                   Languages.GeneralAccept);
                return;
            }
            if (string.IsNullOrEmpty(CarPlate))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Your number car should not be empty",
                   Languages.GeneralAccept);
                return;
            }

            IsRunning = true;
            IsEnabled = false;
            var userResponse = await InsideApi.GetUserByUserName(HostSetting.AuthEndPoint, MainViewModel.GetInstance().CurrentUser.UserName);

            var model = new EditProfileViewModel
            {
                Id = ((UserModel)userResponse.Result).Id,
                UserName = UserName,
                Name = Name,
                OldPassword = OldPassword,
                NewPassword = NewPassword,
                Email = Email,
                Lastname = Lastname,
                Address = Address,
                State = State,
                CarPlate = CarPlate
            };

            var response = await InsideApi.Post<EditProfileViewModel, TokenResponse>(
                HostSetting.EditProfileEndPoint,
                model);

            if (response == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.GeneralError,
                    Languages.LoginErrorServerAlert +
                    "\n" +
                   Languages.GeneralCheckInternetConnection +
                    "\n",
                   Languages.GeneralAccept);
            }
            else
            {
                if (response.IsSuccess)
                {
                    await Application.Current.MainPage.DisplayAlert(
                       Languages.GeneralSuccess,
                       Languages.EditProfileSuccessAlert,
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

