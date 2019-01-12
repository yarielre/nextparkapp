using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using NextPark.Models;
using NextPark.Mobile.Core.Settings;

namespace NextPark.Mobile.ViewModels
{
	public class MoneyViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header user money value
        public ICommand OnMoneyClick { get; set; }  // Header user money action

        public string Earn { get; set; }                // User Earn value
        public Boolean Btn05IsSelected { get; set; }    //  5 CHF button selected
        public Boolean Btn10IsSelected { get; set; }    // 10 CHF button selected
        public Boolean Btn15IsSelected { get; set; }    // 15 CHF button selected
        public Boolean Btn30IsSelected { get; set; }    // 30 CHF button selected
        public ICommand OnButtonTapped { get; set; }    // Selection button tapped

        public bool IsRunning { get; set; }         // Activity spinner
        public ICommand OnBuyClick { get; set; }    // Buy button action

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IProfileService _profileService;

        // PRIVATE VARIABLES
        protected static UInt16 selectedValue;
        private UpdateUserCoinModel UpdateUserCoin;

        // METHODS
        public MoneyViewModel(IDialogService dialogService,
                              IApiService apiService,
                              IAuthService authService,
                              INavigationService navService)
                              : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _profileService = new ProfileService(apiService);

            // Header
            // TODO: evaluate back text and action
            BackText = "Profilo";
            UserName = AuthSettings.UserName;
            UserMoney = AuthSettings.UserCoin.ToString("N0");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            // Page actions
            OnButtonTapped = new Command<string>(OnButtonTappedMethod);
            OnBuyClick = new Command<object>(OnBuyClickMethod);
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            /*
            if (data != null)
            {
                return Task.FromResult(false);
            }
            */

            // Header
            // TODO: evaluate back text and action
            BackText = "Profilo";
            UserName = AuthSettings.UserName;
            UserMoney = AuthSettings.UserCoin.ToString("N0");

            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Budget Values
            // User Money already updated for header
            Earn = "0";
            base.OnPropertyChanged("Earn");

            // Buttons start/default value
            Btn05IsSelected = true;
            Btn10IsSelected = false;
            Btn15IsSelected = false;
            Btn30IsSelected = false;
            base.OnPropertyChanged("Btn05IsSelected");
            base.OnPropertyChanged("Btn10IsSelected");
            base.OnPropertyChanged("Btn15IsSelected");
            base.OnPropertyChanged("Btn30IsSelected");

            selectedValue = 5;

            return Task.FromResult(false);
        }

        // Back Click Action
        public void OnBackClickMethod(object sender)
        {
            // TODO: evaluate back action (profile or previous view?) 
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            // Already in MoneyPage
        }

        // Selection Button Tapped action
        public void OnButtonTappedMethod(string identifier)
        {
            // Deselect all selection buttons
            Btn05IsSelected = false;            
            Btn10IsSelected = false;
            Btn15IsSelected = false;
            Btn30IsSelected = false;

            // Select the tapped selection button
            selectedValue = Convert.ToUInt16(identifier);
            switch (selectedValue)
            {
                case 10: Btn10IsSelected = true; break; 
                case 15: Btn15IsSelected = true; break;
                case 30: Btn30IsSelected = true; break;
                case 5:
                default: Btn05IsSelected = true; break;
            }

            // Update Buttons
            base.OnPropertyChanged("Btn05IsSelected");
            base.OnPropertyChanged("Btn10IsSelected");
            base.OnPropertyChanged("Btn15IsSelected");
            base.OnPropertyChanged("Btn30IsSelected");
        }
    
        // Buy Money button click action
        public void OnBuyClickMethod(object sender)
        {
            // TODO: fill data according to buy credit data model
            // TODO: send buy credit request to backend
            _dialogService.ShowAlert("Alert", "TODO: Payment operations for: " + selectedValue.ToString() + " CHF");

            UpdateUserCoin = new UpdateUserCoinModel { Coins = AuthSettings.UserCoin + double.Parse(selectedValue.ToString()), UserId = int.Parse(AuthSettings.UserId) };

            // Start activity spinner
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");

            // Send request to backend
            BuyMoney();
        } 

        public async void BuyMoney()
        {
            if (UpdateUserCoin != null)
            {
                var buyResponse = await _profileService.UpdateUserCoins(UpdateUserCoin);

                // Stop activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");

                // Check update result
                if (buyResponse != null) {
                    AuthSettings.UserCoin = buyResponse.Coins;
                    UserMoney = AuthSettings.UserCoin.ToString("N0");
                    base.OnPropertyChanged("UserMoney");
                } else {
                    await _dialogService.ShowAlert("Attenzione", "Acquisto fallito");
                }
            } else {
                await _dialogService.ShowAlert("Errore", "Dati non validi");
            }
        }
    }
}
