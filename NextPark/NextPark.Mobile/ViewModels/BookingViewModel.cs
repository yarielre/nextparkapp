using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System;
using NextPark.Mobile.Settings;
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
        public string Notes { get; set; }               // Parking notes text
        public bool IsNotesVisible { get; set; }        // Parking notes visibility
        public string Picture { get; set; }             // Parking picture source text
        public Aspect PictureAspect { get; set; }       // Parking picture aspect
        public string FullPrice { get; set; }           // Parking price full text (2 CHF/h)
        public string FullAvailability { get; set; }    // Parking availability full text (08:00-10:00)
        public TimeSpan Time { get; set; }              // Booking time text

        public bool IsRunning { get; set; }             // Activity spinner

        public ICommand TimeChanged { get; set; }       // Time Picker property changed
        public ICommand BookAction { get; set; }        // Time Picker property changed

        // Buttons
        public Boolean Btn1IsSelected { get; set; }     // 0.5h button selected
        public Boolean Btn1IsEnabled { get; set; }      // 0.5h button enabled
        public string Btn1SubInfo { get; set; }         // 0.5h button price
        public Boolean Btn2IsSelected { get; set; }     // 1.0h button selected
        public Boolean Btn2IsEnabled { get; set; }      // 1.0h button enabled
        public string Btn2SubInfo { get; set; }         // 1.0h button price
        public Boolean Btn3IsSelected { get; set; }     // 2.0h button selected
        public Boolean Btn3IsEnabled { get; set; }      // 2.0h button enabled
        public string Btn3SubInfo { get; set; }         // 2.0h button price
        public Boolean Btn4IsSelected { get; set; }     // 3.0h button selected
        public Boolean Btn4IsEnabled { get; set; }      // 3.0h button enabled
        public string Btn4SubInfo { get; set; }         // 3.0h button price
        public Boolean Btn5IsSelected { get; set; }     // 4.0h button selected
        public Boolean Btn5IsEnabled { get; set; }      // 4.0h button enabled
        public string Btn5SubInfo { get; set; }         // 4.0h button price
        public Boolean Btn6IsSelected { get; set; }     // 5.0h button selected
        public Boolean Btn6IsEnabled { get; set; }      // 5.0h button enabled
        public string Btn6SubInfo { get; set; }         // 5.0h button price
        public Boolean Btn7IsSelected { get; set; }     // 6.0h button selected
        public Boolean Btn7IsEnabled { get; set; }      // 6.0h button enabled
        public string Btn7SubInfo { get; set; }         // 6.0h button price
        public Boolean Btn8IsSelected { get; set; }     // 8.0h button selected
        public Boolean Btn8IsEnabled { get; set; }      // 8.0h button enabled
        public string Btn8SubInfo { get; set; }         // 8.0h button price
        public ICommand OnButtonTapped { get; set; }    // Selection button tapped

        // Confirm pop-up
        public bool ConfirmVisible { get; set; }
        public string ConfirmStartDateTime { get; set; }
        public string ConfirmEndDateTime { get; set; }
        public string ConfirmPrice { get; set; }
        public ICommand OnConfirm { get; set; }
        public ICommand OnCancel { get; set; }

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IOrderDataService _orderDataService;

        // PRIVATE VARIABLES
        private UIParkingModel _parking;
        private OrderModel _order;

        // METHODS
        public BookingViewModel(IDialogService dialogService,
                                IApiService apiService,
                                IAuthService authService,
                                INavigationService navService,
                                IOrderDataService orderDataService)
                                : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _orderDataService = orderDataService;

            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N2");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            OnButtonTapped = new Command<string>(OnButtonTappedMethod);
            BookAction = new Command<object>(OnBookingMethod);

            ConfirmVisible = false;
            OnConfirm = new Command(OnConfirmMethod);
            OnCancel = new Command(OnCancelMethod);
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
            UserMoney = AuthSettings.UserCoin.ToString("N2");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            if (data is UIParkingModel)
            {
                _parking = (UIParkingModel)data;
                Info = _parking.Address;
                SubInfo = _parking.Cap.ToString() + " " + _parking.City;
                if (string.IsNullOrEmpty(_parking.Notes))
                {
                    Notes = "";
                    IsNotesVisible = false;
                } else
                {
                    Notes = _parking.Notes;
                    IsNotesVisible = true;
                }

                if (string.IsNullOrEmpty(_parking.ImageUrl))
                {
                    Picture = "icon_no_photo.png";
                    PictureAspect = Aspect.AspectFit;
                }
                else
                {
                    Picture = ApiSettings.BaseUrl + _parking.ImageUrl;
                    PictureAspect = Aspect.AspectFill;
                }
                FullPrice = _parking.PriceMin.ToString("N2") + " CHF/h";
                FullAvailability = (_parking.isFree()) ? "Disponibile" : "Occupato";
                base.OnPropertyChanged("Info");
                base.OnPropertyChanged("SubInfo");
                base.OnPropertyChanged("Notes");
                base.OnPropertyChanged("IsNotesVisible");
                base.OnPropertyChanged("Picture");
                base.OnPropertyChanged("PictureAspect");
                base.OnPropertyChanged("FullPrice");
                base.OnPropertyChanged("FullAvailability");

                Btn1SubInfo = (_parking.PriceMin * 0.5).ToString("N2") + "\nCHF";
                Btn2SubInfo = (_parking.PriceMin * 1.0).ToString("N2") + "\nCHF";
                Btn3SubInfo = (_parking.PriceMin * 2.0).ToString("N2") + "\nCHF";
                Btn4SubInfo = (_parking.PriceMin * 3.0).ToString("N2") + "\nCHF";
                Btn5SubInfo = (_parking.PriceMin * 4.0).ToString("N2") + "\nCHF";
                Btn6SubInfo = (_parking.PriceMin * 5.0).ToString("N2") + "\nCHF";
                Btn7SubInfo = (_parking.PriceMin * 6.0).ToString("N2") + "\nCHF";
                Btn8SubInfo = (_parking.PriceMin * 8.0).ToString("N2") + "\nCHF";

                base.OnPropertyChanged("Btn1SubInfo");
                base.OnPropertyChanged("Btn2SubInfo");
                base.OnPropertyChanged("Btn3SubInfo");
                base.OnPropertyChanged("Btn4SubInfo");
                base.OnPropertyChanged("Btn5SubInfo");
                base.OnPropertyChanged("Btn6SubInfo");
                base.OnPropertyChanged("Btn7SubInfo");
                base.OnPropertyChanged("Btn8SubInfo");
            }

            TimeSpan availableTime = _parking.GetAvailableTime(DateTime.Now);

            // Enable buttons
            Btn1IsEnabled = true;
            Btn2IsEnabled = true;
            Btn3IsEnabled = true;
            Btn4IsEnabled = true;
            Btn5IsEnabled = true;
            Btn6IsEnabled = true;
            Btn7IsEnabled = true;
            Btn8IsEnabled = true;

            for (int i = 8; i > 0; i--) {
                if (availableTime > GetButtonTime(i)) {
                    break;
                }
                switch (i)
                {
                    case 1: Btn1IsEnabled = false; break;
                    case 2: Btn2IsEnabled = false; break;
                    case 3: Btn3IsEnabled = false; break;
                    case 4: Btn4IsEnabled = false; break;
                    case 5: Btn5IsEnabled = false; break;                    
                    case 6: Btn6IsEnabled = false; break;
                    case 7: Btn7IsEnabled = false; break;
                    case 8: Btn8IsEnabled = false; break;
                }
            }

            // Buttons start/default value
            Btn1IsSelected = false;
            Btn2IsSelected = false;
            Btn3IsSelected = false;
            Btn4IsSelected = false;
            Btn5IsSelected = false;
            Btn6IsSelected = false;
            Btn7IsSelected = false;
            Btn8IsSelected = false;

            if (availableTime > TimeSpan.FromHours(1)) {
                // Parking available for at least 1h, set as start selection
                Btn2IsSelected = true;
                Time = TimeSpan.FromHours(1.0);                
            } else if (availableTime > TimeSpan.FromMinutes(30)) {
                // Parking available for at least 30 minutes, set as start selection
                Btn1IsSelected = true;
                Time = TimeSpan.FromMinutes(30.0);
            } else {
                // Try to set maximum available time
                Time = availableTime;
            }
            base.OnPropertyChanged("Btn1IsSelected");
            base.OnPropertyChanged("Btn2IsSelected");
            base.OnPropertyChanged("Btn3IsSelected");
            base.OnPropertyChanged("Btn4IsSelected");
            base.OnPropertyChanged("Btn5IsSelected");
            base.OnPropertyChanged("Btn6IsSelected");
            base.OnPropertyChanged("Btn7IsSelected");
            base.OnPropertyChanged("Btn8IsSelected");

            base.OnPropertyChanged("Btn1IsEnabled");
            base.OnPropertyChanged("Btn2IsEnabled");
            base.OnPropertyChanged("Btn3IsEnabled");
            base.OnPropertyChanged("Btn4IsEnabled");
            base.OnPropertyChanged("Btn5IsEnabled");
            base.OnPropertyChanged("Btn6IsEnabled");
            base.OnPropertyChanged("Btn7IsEnabled");
            base.OnPropertyChanged("Btn8IsEnabled");

            base.OnPropertyChanged("Time");

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
            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

            // Compute price
            double orderPrice = Time.TotalHours * _parking.PriceMin;
            if (_parking.UserId == AuthSettings.User.Id) {
                orderPrice = 0;
            }
            // Check user balance
            if (AuthSettings.User.Balance < orderPrice) {
                // Not enough credit
                _dialogService.ShowAlert("Attenzione", "Credito insufficiente");
                NavigationService.NavigateToAsync<MoneyViewModel>();
                return;
            }

            // TODO: fill book data according to add book backend method

            _order = new OrderModel
            {
                ParkingId = _parking.Id,
                StartDate = now,
                EndDate = now + Time,
                Price = orderPrice,
                UserId = int.Parse(AuthSettings.UserId),
                CarPlate = AuthSettings.User.CarPlate
            };

            // Show activity spinner
            this.IsRunning = true;
            base.OnPropertyChanged("IsRunning");

            // Ask confirm
            ConfirmStartDateTime = _order.StartDate.ToString("dd/MM/yy HH:mm");
            ConfirmEndDateTime = _order.EndDate.ToString("dd/MM/yy HH:mm");
            ConfirmPrice = _order.Price.ToString("N2") + " CHF";
            ConfirmVisible = true;

            base.OnPropertyChanged("ConfirmStartDateTime");
            base.OnPropertyChanged("ConfirmEndDateTime");
            base.OnPropertyChanged("ConfirmPrice");
            base.OnPropertyChanged("ConfirmVisible");
        }

        // Selection Button Tapped action
        public void OnButtonTappedMethod(string identifier)
        {
            UInt16 selectedValue;
            selectedValue = Convert.ToUInt16(identifier);

            if (IsButtonEnabled(selectedValue))
            {
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
        }

        public TimeSpan GetButtonTime(int button)
        {
            switch (button)
            {
                case 1: return TimeSpan.FromMinutes(30);
                case 2: return TimeSpan.FromHours(1.0);
                case 3: return TimeSpan.FromHours(2.0);
                case 4: return TimeSpan.FromHours(3.0);
                case 5: return TimeSpan.FromHours(4.0);
                case 6: return TimeSpan.FromHours(5.0);
                case 7: return TimeSpan.FromHours(6.0);
                case 8: return TimeSpan.FromHours(8.0);                
                default: return TimeSpan.FromMinutes(0.0);
            }
        }

        public bool IsButtonEnabled(int button)
        {
            switch (button)
            {
                case 1: return Btn1IsEnabled;
                case 2: return Btn2IsEnabled;
                case 3: return Btn3IsEnabled;
                case 4: return Btn4IsEnabled;
                case 5: return Btn5IsEnabled;
                case 6: return Btn6IsEnabled;
                case 7: return Btn7IsEnabled;
                case 8: return Btn8IsEnabled;                
                default: return false;
            }
        }

        public async void SendOrder(OrderModel order)
        {
            try
            {
                var result = await _orderDataService.CreateOrderAsync(order);

                // Hide activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");

                if (result != null)
                {
                    if (result.IsSuccess == true)
                    {
                        // Successful
                        await NavigationService.NavigateToAsync<UserBookingViewModel>();
                    }
                    else if (result.ErrorType == Enums.Enums.ErrorType.NotEnoughMoney)
                    {
                        // Not enough credit
                        await _dialogService.ShowAlert("Attenzione", "Credito insufficiente");
                        await NavigationService.NavigateToAsync<MoneyViewModel>();
                        return;
                    }
                    else if ((result.ErrorType == Enums.Enums.ErrorType.ParkingNotOrderable) || (result.ErrorType == Enums.Enums.ErrorType.ParkingNotVailable))
                    {
                        // Parking not available
                        await _dialogService.ShowAlert("Attenzione", "Il parcheggio non è più disponibile");
                        await NavigationService.NavigateToAsync<HomeViewModel>();
                        return;
                    } else {
                        // Unexpected error
                        await _dialogService.ShowAlert("Errore", "Impossibile eseguire l'ordine");
                        await NavigationService.NavigateToAsync<HomeViewModel>();
                        return;
                    }
                } else {
                    // Unexpected error
                    await _dialogService.ShowAlert("Errore", "Impossibile eseguire l'ordine");
                    await NavigationService.NavigateToAsync<HomeViewModel>();
                    return;
                }
            } catch (Exception e) {
                await _dialogService.ShowAlert("Errore", e.Message);
            }
        }

        public void OnConfirmMethod()
        {
            ConfirmVisible = false;
            base.OnPropertyChanged("ConfirmVisible");
            SendOrder(_order);
        }

        public void OnCancelMethod()
        {
            ConfirmVisible = false;
            base.OnPropertyChanged("ConfirmVisible");
            // Hide activity spinner
            IsRunning = false;
            base.OnPropertyChanged("IsRunning");
        }

    }
}
