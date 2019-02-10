using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System;
using NextPark.Mobile.Core.Settings;
using NextPark.Mobile.Services.Data;
using NextPark.Models;
using NextPark.Mobile.UIModels;

namespace NextPark.Mobile.ViewModels
{
    public class BookingViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string Info { get; set; }                // Parking info text
        public string SubInfo { get; set; }             // Parking subInfo text
        public string Picture { get; set; }             // Parking picture source text
        public string FullPrice { get; set; }           // Parking price full text (2 CHF/h)
        public string FullAvailability { get; set; }    // Parking availability full text (08:00-10:00)
        public TimeSpan Time { get; set; }                // Booking time text

        public bool IsRunning { get; set; }         // Activity spinner

        public ICommand TimeChanged { get; set; }   // Time Picker property changed
        public ICommand BookAction { get; set; }   // Time Picker property changed

        // Buttons
        public Boolean Btn1IsSelected { get; set; }     // 0.5h button selected
        public Boolean Btn2IsSelected { get; set; }     // 1.0h button selected
        public Boolean Btn3IsSelected { get; set; }     // 2.0h button selected
        public Boolean Btn4IsSelected { get; set; }     // 3.0h button selected
        public Boolean Btn5IsSelected { get; set; }     // 4.0h button selected
        public Boolean Btn6IsSelected { get; set; }     // 5.0h button selected
        public Boolean Btn7IsSelected { get; set; }     // 6.0h button selected
        public Boolean Btn8IsSelected { get; set; }     // 8.0h button selected
        public ICommand OnButtonTapped { get; set; }    // Selection button tapped

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly OrderDataService _orderDataService;

        // PRIVATE VARIABLES
        private static bool activity = false;
        private UIParkingModel _parking;

        // METHODS
        public BookingViewModel(IDialogService dialogService,
                                IApiService apiService,
                                IAuthService authService,
                                INavigationService navService,
                                OrderDataService orderDataService)
                                : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _orderDataService = orderDataService;

            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            OnButtonTapped = new Command<string>(OnButtonTappedMethod);
            BookAction = new Command<object>(OnBookingMethod);
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            if (data == null)
            {
                return Task.FromResult(false);
            }

            // Header
            BackText = "Mappa";
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            if (data is UIParkingModel)
            {
                _parking = (UIParkingModel)data;
                Info = _parking.Address;
                SubInfo = _parking.Cap.ToString() + " " + _parking.City;
                Picture = _parking.ImageUrl;
                FullPrice = _parking.PriceMin.ToString("N2") + "CHF/h";
                FullAvailability = (_parking.isFree()) ? "Disponibile" : "Occupato";
                base.OnPropertyChanged("Info");
                base.OnPropertyChanged("SubInfo");
                base.OnPropertyChanged("Picture");
                base.OnPropertyChanged("FullPrice");
                base.OnPropertyChanged("FullAvailability");

                Time = TimeSpan.FromHours(1.0);
                base.OnPropertyChanged("Time");
            }

            // Buttons start/default value
            Btn1IsSelected = false;
            Btn2IsSelected = true;
            Btn3IsSelected = false;
            Btn4IsSelected = false;
            Btn5IsSelected = false;
            Btn6IsSelected = false;
            Btn7IsSelected = false;
            Btn8IsSelected = false;
            base.OnPropertyChanged("Btn1IsSelected");
            base.OnPropertyChanged("Btn2IsSelected");
            base.OnPropertyChanged("Btn3IsSelected");
            base.OnPropertyChanged("Btn4IsSelected");
            base.OnPropertyChanged("Btn5IsSelected");
            base.OnPropertyChanged("Btn6IsSelected");
            base.OnPropertyChanged("Btn7IsSelected");
            base.OnPropertyChanged("Btn8IsSelected");

            return Task.FromResult(false);
        }

        // Back Click Action
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<HomeViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // Booking button click action
        public void OnBookingMethod(object sender)
        {
            string message = string.Format("Prenota per {0} h", Time.TotalHours);
            _dialogService.ShowAlert("Alert", message);

            // TODO: execute payment
            // TODO: fill book data according to add book backend method
            OrderModel order = new OrderModel
            {
                ParkingId = _parking.Id,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now + Time,
                UserId = int.Parse(AuthSettings.UserId)
            };

            // Show activity spinner
            if (activity == true)
            {
                activity = false;
            }
            else
            {
                activity = true;
            }
            this.IsRunning = activity;
            base.OnPropertyChanged("IsRunning");

            // TODO: send book action to backend
            SendOrder(order);
        }

        // Selection Button Tapped action
        public void OnButtonTappedMethod(string identifier)
        {
            UInt16 selectedValue;

            // Deselect all selection buttons
            Btn1IsSelected = false;
            Btn2IsSelected = false;
            Btn3IsSelected = false;
            Btn4IsSelected = false;
            Btn5IsSelected = false;
            Btn6IsSelected = false;
            Btn7IsSelected = false;
            Btn8IsSelected = false;

            // Select the tapped selection button
            selectedValue = Convert.ToUInt16(identifier);
            switch (selectedValue)
            {
                case 2: Btn2IsSelected = true; Time = TimeSpan.FromHours(1.0); break;
                case 3: Btn3IsSelected = true; Time = TimeSpan.FromHours(2.0); break;
                case 4: Btn4IsSelected = true; Time = TimeSpan.FromHours(3.0); break;
                case 5: Btn5IsSelected = true; Time = TimeSpan.FromHours(4.0); break;
                case 6: Btn6IsSelected = true; Time = TimeSpan.FromHours(5.0); break;
                case 7: Btn7IsSelected = true; Time = TimeSpan.FromHours(6.0); break;
                case 8: Btn8IsSelected = true; Time = TimeSpan.FromHours(8.0); break;
                case 1:
                default: Btn1IsSelected = true; Time = TimeSpan.FromMinutes(30.0); break;
            }

            // Update Buttons
            base.OnPropertyChanged("Btn1IsSelected");
            base.OnPropertyChanged("Btn2IsSelected");
            base.OnPropertyChanged("Btn3IsSelected");
            base.OnPropertyChanged("Btn4IsSelected");
            base.OnPropertyChanged("Btn5IsSelected");
            base.OnPropertyChanged("Btn6IsSelected");
            base.OnPropertyChanged("Btn7IsSelected");
            base.OnPropertyChanged("Btn8IsSelected");

            base.OnPropertyChanged("Time");
        }

        public async void SendOrder(OrderModel order)
        {
            try {
                var result = await _orderDataService.Post(order);

                IsRunning = false;
                base.OnPropertyChanged("IsRunning");

                if (result != null) {
                    await NavigationService.NavigateToAsync<UserBookingViewModel>();
                } else {
                    await _dialogService.ShowAlert("Errore", "Impossibile eseguire l'ordine");
                }
            } catch (Exception e) {
                await _dialogService.ShowAlert("Errore", e.Message);
            }
        }
    }
}
