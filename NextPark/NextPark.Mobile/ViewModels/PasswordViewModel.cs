using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using NextPark.Models;
using System.Text.RegularExpressions;
using Microsoft.AppCenter;
using NextPark.Enums.Enums;
using NextPark.Mobile.Settings;

namespace NextPark.Mobile.ViewModels
{
    public class PasswordViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header username
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string CurrentPassword { get; set; } // Current Password text
        public string NewPassword { get; set; }     // New Password text
        public string PasswordConfirm { get; set; } // Password confirm text

        public bool IsRunning { get; set; }             // Activity spinner
        public ICommand OnChangePasswordClick { get; set; }   // Register button action

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IProfileService _profileService;


        // PRIVATE VARIBLES
        private bool dataCheckError;

        public PasswordViewModel(IDialogService dialogService,
                                 IProfileService profileService,
                                 IApiService apiService,
                                 IAuthService authService,
                                 INavigationService navService)
                                 : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _profileService = profileService;

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnChangePasswordClick = new Command<object>(OnChangePasswordClickMethod);
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            // Header
            BackText = "Indietro";
            UserName = "Accedi";
            UserMoney = "0";
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            return Task.FromResult(false);
        }

        public override bool BackButtonPressed()
        {
            OnBackClickMethod(null);
            return false; // Do not propagate back button pressed
        }

        // Back Click action
        public void OnBackClickMethod(object sender)
        {
            // TODO: go back to previus view
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            // TODO: evaluate action (try to go to profile, do nothing?)
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            // TODO: evaluate action (try to go to money page, do nothing?)
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // Register button action
        public void OnChangePasswordClickMethod(object sender)
        {
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");
            ChangePasswordMethod();
        }

        private async Task<bool> ChangePasswordDataCheck()
        {
            bool error = false;

            // Current Password
            if (string.IsNullOrEmpty(this.CurrentPassword))
            {
                // No current password inserted
                await _dialogService.ShowAlert("Avviso", "Inserire la password attuale");
                error = true;
            }
            // New
            if (!error && ((string.IsNullOrEmpty(this.NewPassword)) || (!Regex.IsMatch(this.NewPassword, @"(?=^.{6,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])"))))
            {
                // No valid password inserted
                await _dialogService.ShowAlert("Avviso", "La password deve rispettare i seguenti parametri:\n - minimo 6 caratteri\n - una lettera maiuscola\n - una lettera minuscola\n - un numero");
                error = true;
            }
            // Password confirm
            if (!error && ((string.IsNullOrEmpty(this.PasswordConfirm)) || (false == this.NewPassword.Equals(this.PasswordConfirm))))
            {
                await _dialogService.ShowAlert("Le password sono differenti", "Il campo password e conferma password devono essere identici.");
                error = true;
            }

            return error;
        }

        public async void ChangePasswordMethod()
        {
            // Change Password data check
            bool error = await ChangePasswordDataCheck();
            if (error)
            {
                // Stop activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
                return;
            }

            //
            try
            {
                var changePasswordModel = new ChangePasswordModel
                {
                    Id = AuthSettings.User.Id,
                    OldPassword = CurrentPassword,
                    NewPassword = NewPassword
                };

                var changePasswordResponse = await _profileService.ChangePassword(changePasswordModel);

                if ((changePasswordResponse == true))
                {
                    await _dialogService.ShowAlert("Avviso", "Password cambiata");
                    // Change password ok, go to home page
                    await NavigationService.NavigateToAsync<UserProfileViewModel>();
                }
                else
                {
                    await _dialogService.ShowAlert("Errore", "Cambio password non riuscito");
                }

                // Stop activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
            }
            catch (Exception ex)
            {
                // Stop activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
            }
        }
    }
}