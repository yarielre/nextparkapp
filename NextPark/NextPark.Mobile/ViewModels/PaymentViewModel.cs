using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Stripe;
using NextPark.Models;
using System.Text.RegularExpressions;
using Microsoft.AppCenter;
using NextPark.Enums.Enums;

namespace NextPark.Mobile.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header username
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string CreditCardNumber { get; set; }    // Credit card number
        public string CardExpirationDate { get; set; }  // Expiration date
        public string CreditCardHolder { get; set; }    // Credit card holder
        public string CVV { get; set; }                 // CVV

        public bool IsRunning { get; set; }             // Activity spinner
        public ICommand OnExecuteClick { get; set; }   // Execute button action

        // SERVICES
        private readonly IDialogService _dialogService;

        // PRIVATE VARIABLES
        private int _amount;
        private int expMonth;
        private int expYear;

        // METHODS
        public PaymentViewModel(IDialogService dialogService,
                                     IApiService apiService,
                                     IAuthService authService,
                                     INavigationService navService)
                                     : base(apiService, authService, navService)
        {
            _dialogService = dialogService;

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnExecuteClick = new Command<object>(OnExecuteClickMethod);
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            if ((data != null) && (data is UInt16))
            {
                _amount = (UInt16)(data) * 100;

            }
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
            NavigationService.NavigateToAsync<LoginViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            // TODO: evaluate action (try to go to profile, do nothing?)
            NavigationService.NavigateToAsync<LoginViewModel>();
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            // TODO: evaluate action (try to go to money page, do nothing?)
            NavigationService.NavigateToAsync<LoginViewModel>();
        }

        // Register button action
        public void OnExecuteClickMethod(object sender)
        {
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");
            ExecutePaymentMethod();
        }

        private async Task<bool> PaymentDataCheck()
        {
            bool error = false;
            
            // Credi card number
            if (!error && (string.IsNullOrEmpty(this.CreditCardNumber)))
            {
                await _dialogService.ShowAlert("Avviso", "Inserire un numero di carta valido");
                error = true;
            }
            // Credi card holder
            if (!error && (string.IsNullOrEmpty(this.CreditCardHolder)))
            {
                await _dialogService.ShowAlert("Avviso", "Inserire il nome del titolare della carta");
                error = true;
            }
            // Credi card expiration date
            if (!error && (string.IsNullOrEmpty(this.CardExpirationDate)))
            {
                await _dialogService.ShowAlert("Avviso", "Inserire la data di scadenza della carta");
                error = true;
            }
            // Card expiration
            if (!error && (!string.IsNullOrEmpty(CardExpirationDate)))
            {
                var test = CardExpirationDate.Substring(0, 2);
                if (int.TryParse(CardExpirationDate.Substring(0, 2), out expMonth))
                {
                    if (int.TryParse(CardExpirationDate.Substring(3, 2), out expYear))
                    {
                        expYear = 2000 + expYear;
                        if ((expYear < DateTime.Now.Year) || ((expYear == DateTime.Now.Year) && (expMonth < DateTime.Now.Month)))
                        {
                            await _dialogService.ShowAlert("Avviso", "Carta scaduta");
                            error = true;
                        }
                    }
                }
            }
            // CVV
            if (!error && (string.IsNullOrEmpty(this.CVV) || (CVV.Length < 3)))
            {
                await _dialogService.ShowAlert("Avviso", "Inserire il codice di sicurezza");
                error = true;
            }
            return error;
        }

        public async void ExecutePaymentMethod()
        {
            // Payment data check
            bool error = await PaymentDataCheck();
            if (error)
            {
                // Stop activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
                return;
            }

            try
            {
                var options = new TokenCreateOptions
                {
                    Card = new CreditCardOptions
                    {
                        Number = CreditCardNumber,
                        ExpYear = Convert.ToInt64(expYear),
                        ExpMonth = Convert.ToInt64(expMonth),
                        Cvc = CVV,
                        Currency = "chf"
                    }
                };

                var service = new TokenService();

                //StripeConfiguration.ApiKey = "pk_test_uh3rNLYEtplli9X2LAR4w1Ql00xTckxLrs";
                StripeConfiguration.ApiKey = "sk_test_Lp7wZ65RQFyrvXjbHd2LBAeC00MxHQlYZO";

                Token stripeToken = service.Create(options);

                var options2 = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt64(_amount),
                    Currency = "chf",
                    Description = "Example",
                    Source = stripeToken.Id,
                    StatementDescriptor = "Example1"
                };
                var service1 = new ChargeService();

                Charge charge = service1.Create(options2);

            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Test", ex.Message);
            }
            finally
            {
                // Stop activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
            }
        }  
    }
}
