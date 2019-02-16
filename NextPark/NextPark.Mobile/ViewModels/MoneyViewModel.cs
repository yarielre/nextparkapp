using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using NextPark.Models;
using NextPark.Mobile.Settings;

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

        public string Money { get; set; }               // User Money value
        public string Profit { get; set; }              // User Profit value
        public Boolean Btn10IsSelected { get; set; }    // 10 CHF button selected
        public Boolean Btn20IsSelected { get; set; }    // 20 CHF button selected
        public Boolean Btn30IsSelected { get; set; }    // 30 CHF button selected
        public Boolean Btn50IsSelected { get; set; }    // 50 CHF button selected
        public ICommand OnButtonTapped { get; set; }    // Selection button tapped

        public bool IsRunning { get; set; }         // Activity spinner

        public bool WithdrawalEnable { get; set; }          // Withdrawal button enable
        public Color BtnWithdrawalBorderColor { get; set; } // Withdrawal button border color
        public Color BtnWithdrawalBackgroundColor { get; set; } // Withdrawal button background color

        public ICommand OnWithdrawalClick { get; set; } // Withdrawal button action
        public ICommand OnBuyClick { get; set; }        // Buy button action

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
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            Money = AuthSettings.User.Balance.ToString("N2");
            Profit = AuthSettings.User.Profit.ToString("N2");
            DisableWithdrawal();

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            // Page actions
            OnButtonTapped = new Command<string>(OnButtonTappedMethod);
            OnBuyClick = new Command<object>(OnBuyClickMethod);
            OnWithdrawalClick = new Command<object>(OnWithdrawalClickMethod);
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            // Header
            // TODO: evaluate back text and action
            BackText = "Profilo";
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Budget Values
            Money = AuthSettings.User.Balance.ToString("N2");
            Profit = AuthSettings.User.Profit.ToString("N2");
            base.OnPropertyChanged("Money");
            base.OnPropertyChanged("Profit");

            // Withdrawal enable
            if (AuthSettings.User.Profit > 10.0) {
                EnableWithdrawal();
            } else {
                DisableWithdrawal();
            }

            // Buttons start/default value
            Btn10IsSelected = true;
            Btn20IsSelected = false;
            Btn30IsSelected = false;
            Btn50IsSelected = false;
            base.OnPropertyChanged("Btn10IsSelected");
            base.OnPropertyChanged("Btn20IsSelected");
            base.OnPropertyChanged("Btn30IsSelected");
            base.OnPropertyChanged("Btn50IsSelected");

            selectedValue = 10;

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
            Btn10IsSelected = false;            
            Btn20IsSelected = false;
            Btn30IsSelected = false;
            Btn50IsSelected = false;

            // Select the tapped selection button
            selectedValue = Convert.ToUInt16(identifier);
            switch (selectedValue)
            {
                case 20: Btn20IsSelected = true; break; 
                case 30: Btn30IsSelected = true; break;
                case 50: Btn50IsSelected = true; break;
                case 10:
                default: Btn10IsSelected = true; break;
            }

            // Update Buttons
            base.OnPropertyChanged("Btn10IsSelected");
            base.OnPropertyChanged("Btn20IsSelected");
            base.OnPropertyChanged("Btn30IsSelected");
            base.OnPropertyChanged("Btn50IsSelected");
        }
    
        private void DisableWithdrawal()
        {
            WithdrawalEnable = false;
            BtnWithdrawalBorderColor = Color.Gray;
            BtnWithdrawalBackgroundColor = Color.FromHex("#E3E3E3");
            base.OnPropertyChanged("WithdrawalEnable");
            base.OnPropertyChanged("BtnWithdrawalBorderColor");
            base.OnPropertyChanged("BtnWithdrawalBackgroundColor");
        }

        private void EnableWithdrawal()
        {
            WithdrawalEnable = true;
            BtnWithdrawalBorderColor = Color.FromHex("#8CC63F");
            BtnWithdrawalBackgroundColor = Color.FromHex("#8CC63F");
            base.OnPropertyChanged("WithdrawalEnable");
            base.OnPropertyChanged("BtnWithdrawalBorderColor");
            base.OnPropertyChanged("BtnWithdrawalBackgroundColor");
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
                    var userResponse = await AuthService.GetUserByUserName(AuthSettings.UserName);
                    if (userResponse.IsSuccess)
                    {
                        UserMoney = AuthSettings.UserCoin.ToString("N0");
                        Money = AuthSettings.User.Balance.ToString("N2");
                        Profit = AuthSettings.User.Profit.ToString("N2");
                        base.OnPropertyChanged("UserMoney");
                        base.OnPropertyChanged("Money");
                        base.OnPropertyChanged("Profit");
                    }
                } else {
                    await _dialogService.ShowAlert("Attenzione", "Acquisto fallito");
                }
            } else {
                await _dialogService.ShowAlert("Errore", "Dati non validi");
            }
        }

        // Withdrawal button click action
        public void OnWithdrawalClickMethod(object sender)
        {
            if (WithdrawalEnable == true)
            {
                // TODO: send withdrawal request to backend
                _dialogService.ShowAlert("Alert", "TODO: Withdrawal operations for: " + AuthSettings.User.Profit.ToString() + " CHF");

                // Start activity spinner
                IsRunning = true;
                base.OnPropertyChanged("IsRunning");

                // Send request to backend
                Withdrawal();
            }
        }

        public async void Withdrawal()
        {
            // Stop activity spinner
            IsRunning = false;
            base.OnPropertyChanged("IsRunning");
        }
    }
}
