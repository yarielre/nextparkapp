using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
	public class MoneyViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;

        public string Earn { get; set; }
        public Boolean Btn05IsSelected { get; set; }
        public Boolean Btn10IsSelected { get; set; }
        public Boolean Btn15IsSelected { get; set; }
        public Boolean Btn30IsSelected { get; set; }

        private static bool activity = false;
        protected static UInt16 selectedValue;

        public MoneyViewModel(IDialogService dialogService,
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

            // Page actions
            OnButtonTapped = new Command<string>(OnButtonTappedMethod);
            OnBuyClick = new Command<object>(OnBuyClickMethod);
        }

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
            UserName = "Jonny";
            UserMoney = "8";
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
        public string BackText { get; set; }
        public ICommand OnBackClick { get; set; }
        public void OnBackClickMethod(object sender)
        {
            // TODO: evaluate back action (profile or previous view?) 
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // User Text and Click action
        public string UserName { get; set; }
        public ICommand OnUserClick { get; set; }
        public void OnUserClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Text and Click action
        public string UserMoney { get; set; }
        public ICommand OnMoneyClick { get; set; }
        public void OnMoneyClickMethod(object sender)
        {
            // Already in MoneyPage
        }

        // On Button Tapped
        public ICommand OnButtonTapped { get; set; }
        public void OnButtonTappedMethod(string info)
        {
            // Deselect all selection buttons
            Btn05IsSelected = false;            
            Btn10IsSelected = false;
            Btn15IsSelected = false;
            Btn30IsSelected = false;

            // Select the tapped selection button
            selectedValue = Convert.ToUInt16(info);
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
    
        // On Buy Money
        public bool IsRunning { get; set; }
        public ICommand OnBuyClick { get; set; }
        public void OnBuyClickMethod(object sender)
        {
            // TODO: fill data according to buy credit data model
            // TODO: send buy credit request to backend
            _dialogService.ShowAlert("Alert", "TODO: Buy credit: " + selectedValue + " CHF");

            if (activity == true) activity = false;
            else activity = true;

            IsRunning = activity;
            base.OnPropertyChanged("IsRunning");
        } 
    }
}
