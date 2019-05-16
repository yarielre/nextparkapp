using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using NextPark.Models;
using NextPark.Mobile.Settings;
using NextPark.Mobile.Services.Data;

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
        private readonly IPurchaseDataService _purchaseDataService;
        private readonly IInAppPurchaseService _inAppPurchaseService;

        // PRIVATE VARIABLES
        protected static UInt16 selectedValue;
        private UpdateUserCoinModel UpdateUserCoin;

        // METHODS
        public MoneyViewModel(IDialogService dialogService,
                              IApiService apiService,
                              IAuthService authService,
                              INavigationService navService,
                              IPurchaseDataService purchaseDataService,
                              IInAppPurchaseService inAppPurchaseService)
                              : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _profileService = new ProfileService(apiService);
            _purchaseDataService = purchaseDataService;
            _inAppPurchaseService = inAppPurchaseService;

            // Header
            // TODO: evaluate back text and action
            BackText = "Profilo";
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N2");
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
            UserMoney = AuthSettings.UserCoin.ToString("N2");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Budget Values
            Money = AuthSettings.User.Balance.ToString("N2");
            Profit = AuthSettings.User.Profit.ToString("N2");
            base.OnPropertyChanged("Money");
            base.OnPropertyChanged("Profit");

            // Withdrawal enable
            if (AuthSettings.User.Profit > 50.0) {
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

        public override bool BackButtonPressed()
        {
            OnBackClickMethod(null);
            return false; // Do not propagate back button pressed
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
            BtnWithdrawalBorderColor = (Color)Application.Current.Resources["NextParkColor1"];
            BtnWithdrawalBackgroundColor = (Color)Application.Current.Resources["NextParkColor1"];
            base.OnPropertyChanged("WithdrawalEnable");
            base.OnPropertyChanged("BtnWithdrawalBorderColor");
            base.OnPropertyChanged("BtnWithdrawalBackgroundColor");
        }

        // Buy Money button click action
        public void OnBuyClickMethod(object sender)
        {
            // Start activity spinner
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");

            var purchaseResult = InAppBuyMoney(selectedValue).Result;

            if (purchaseResult.IsSuccess) {
                // Send request to backend
                BuyMoney();
            } else {
                if (purchaseResult.ErrorType == Enums.Enums.ErrorType.InAppPurchaseNotSupported) {
                    _dialogService.ShowAlert("Errore", "L'acquisto In-App non è supportato");
                } else if (purchaseResult.ErrorType == Enums.Enums.ErrorType.InAppPurchaseServiceConnectionError) {
                    _dialogService.ShowAlert("Errore", "Impossibile collegarsi al servizio");
                } else if (purchaseResult.ErrorType == Enums.Enums.ErrorType.InAppPurchaseServiceImposibleToPurchase) {
                    _dialogService.ShowAlert("Errore", "Non è stato possibile completare l'acquisto");
                } else if (purchaseResult.ErrorType == Enums.Enums.ErrorType.InAppPurchaseServiceSuccessPurchase) {
                    // Send request to backend
                    BuyMoney();
                }
            }
        } 

        public async Task<ApiResponse> InAppBuyMoney(ushort value)
        {
            ApiResponse result = new ApiResponse
            {
                IsSuccess = false,
                ErrorType = Enums.Enums.ErrorType.InAppPurchaseServiceImposibleToPurchase,
                Message = "Incorrect value"
            };

            switch(value) {
                case 20:
                    result = await  _inAppPurchaseService.PurchaseCredit20();
                    break;
                case 30:
                    result = await _inAppPurchaseService.PurchaseCredit30();
                    break;
                case 50:
                    result = await _inAppPurchaseService.PurchaseCredit50();
                    break;
                case 10:
                    result = await _inAppPurchaseService.PurchaseCredit1();
                    break;
            }
            await _dialogService.ShowAlert("Alert", "TODO: Payment operations result: " + result.Message);
            return result;
        }

        public async void BuyMoney()
        {
            PurchaseModel purchaseModel = new PurchaseModel
            {
                Cash = double.Parse(selectedValue.ToString()),
                UserId = AuthSettings.User.Id
            };

            var buyResponse = await _purchaseDataService.BuyAmount(purchaseModel);

                 // Stop activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");

                // Check update result
                if (buyResponse != null) {
                    var userResponse = await AuthService.GetUserByUserName(AuthSettings.UserName);
                    if (userResponse.IsSuccess)
                    {
                        UserMoney = AuthSettings.UserCoin.ToString("N2");
                        Money = AuthSettings.User.Balance.ToString("N2");
                        Profit = AuthSettings.User.Profit.ToString("N2");
                        base.OnPropertyChanged("UserMoney");
                        base.OnPropertyChanged("Money");
                        base.OnPropertyChanged("Profit");
                    }
                } else {
                    await _dialogService.ShowAlert("Attenzione", "Acquisto fallito");
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
            try {
                PurchaseModel purchaseModel = new PurchaseModel
                {
                    UserId = AuthSettings.User.Id
                };

                var drawalResult = await _purchaseDataService.DrawalCash(purchaseModel);
                if (drawalResult != null) {
                    // Request accepted
                } else {
                    // Error during drawal operation
                }

            } catch (Exception e) {
                return;
            }
            // Stop activity spinner
            IsRunning = false;
            base.OnPropertyChanged("IsRunning");
        }
    }
}
