using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Inside.Domain.Models;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Models;
using Inside.Xamarin.Services;
using Inside.Xamarin.Views.Tabs;
using Xamarin.Forms;

namespace Inside.Xamarin.ViewModels
{
    public class RegisterUserViewModel : BaseViewModel
    {
        #region Attributes
        private string _password;
        private string _passwordComfirm;
        public string _username;
        public string _email;
        public string _name;
        public string _lastname;
        public string _address;
        public string _state;
        public string _carPlate;
        private bool _isRunning;
        private bool _isEnabled;

        #endregion


        public RegisterUserViewModel()
        {
            IsEnabled = true;
        }

        #region Properties
        public string PasswordConfirm
        {
            get => _passwordComfirm;
            set => SetValue(ref _passwordComfirm, value);
        }
        public string Password
        {
            get => _password;
            set => SetValue(ref _password, value);
        }
        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value);
        }
        public string Lastname
        {
            get => _lastname;
            set => SetValue(ref _lastname, value);
        }
        public string Address
        {
            get => _address;
            set => SetValue(ref _address, value);
        }
        public string State
        {
            get => _state;
            set => SetValue(ref _state, value);
        }
        public string CarPlate
        {
            get => _carPlate;
            set => SetValue(ref _carPlate, value);
        }
        public string UserName
        {
            get => _username;
            set => SetValue(ref _username, value);
        }
        public string Email
        {
            get => _email;
            set => SetValue(ref _email, value);
        }
        public bool IsRunning
        {
            get => _isRunning;
            set => SetValue(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }
        #endregion

        #region Commands
        public ICommand RegisterCommand => new RelayCommand(Register);
        #endregion
        #region Methods
        private async void Register()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.GeneralError,
                    Languages.RegisterUserEmptyAlert,
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
            if (string.IsNullOrEmpty(PasswordConfirm) || !string.Equals(Password, PasswordConfirm))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.GeneralPasswordError,
                    Languages.RegisterPasswordConfAlert,
                    Languages.GeneralAccept);
                PasswordConfirm = string.Empty;
                Password = string.Empty;
                return;
            }

            var model = new RegisterModel
            {
                Username = UserName,
                Password = Password,
                Email = Email,
                Address = Address,
                CarPlate = CarPlate,
                Lastname = Lastname,
                Name = Name,
                State = State
            };
            IsRunning = true;
            IsEnabled = false;
            var response = await InsideApi.Post<RegisterModel, TokenResponse>(HostSetting.RegisterUserEndPoint, model);

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
                try
                {

                    if (response.IsSuccess)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                         Languages.GeneralSuccess,
                         Languages.RegisterSuccess,
                         Languages.GeneralAccept);
                        CleanFields();
                        await NavigationService.GetInstance().NavigateOnLogin(Pages.LoginView);
                    }

                    else
                        await Application.Current.MainPage.DisplayAlert(
                            Languages.GeneralError,
                            Languages.RegisterAddUser,
                            Languages.GeneralAccept);
                }
                catch (Exception e)
                {

                    await Application.Current.MainPage.DisplayAlert(
                          Languages.GeneralError,
                          e.Message,
                          Languages.GeneralAccept);
                }

            }
            IsRunning = false;
            IsEnabled = true;
        }

        private void CleanFields()
        {
            Name = String.Empty;
            UserName = String.Empty;
            Lastname = String.Empty;
            Password = String.Empty;
            PasswordConfirm = String.Empty;
            Email = String.Empty;
            Address = String.Empty;
            CarPlate = String.Empty;
            State = String.Empty;
        }
        #endregion

    }
}