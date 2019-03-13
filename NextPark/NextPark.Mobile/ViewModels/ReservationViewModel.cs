using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NextPark.Domain.Entities;
using NextPark.Mobile.Services;
using NextPark.Mobile.Services.Data;
using NextPark.Mobile.Settings;
using NextPark.Mobile.UIModels;
using NextPark.Models;
using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
    public class ReservationViewModel : BaseViewModel
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
        public Aspect PictureAspect { get; set; }       // Parking picture aspect
        public string FullPrice { get; set; }           // Parking price full text (2 CHF/h)
        public string FullAvailability { get; set; }    // Parking availability full text (08:00-10:00)
        private DateTime _startDate { get; set; }       // Reservation start date 
        public DateTime StartDate {
            get { return _startDate; }
            set { _startDate = value; OnStartDateChanged(); } 
        }         
        public TimeSpan StartTime { get; set; }         // Reservation start time
        public DateTime MinStartDate { get; set; }      // Reservation minimum start date
        public DateTime EndDate { get; set; }           // Reservation end date
        public TimeSpan EndTime { get; set; }           // Reservation end time
        public DateTime MinEndDate { get; set; }        // Reservation minimum end date

        public bool IsRunning { get; set; }         // Activity spinner

        public ICommand TimeChanged { get; set; }   // Time Picker property changed
        public ICommand BookAction { get; set; }   // Time Picker property changed

        // Confirm pop-up
        public bool ConfirmVisible { get; set; }
        public string ConfirmStartDateTime { get; set; }
        public string ConfirmEndDateTime { get; set; }
        public string ConfirmPrice { get; set; }
        public ICommand OnConfirm { get; set; }
        public ICommand OnCancel { get; set; }

        // PRIVATE VARIBLES
        private UIParkingModel _parking;
        private OrderModel _order;

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IOrderDataService _orderDataService;
        private readonly IProfileService _profileService;

        public ReservationViewModel(IDialogService dialogService,
                                    IApiService apiService,
                                    IAuthService authService,
                                    INavigationService navService,
                                    IOrderDataService orderDataService,
                                    IProfileService profileService)
                                : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _orderDataService = orderDataService;
            _profileService = profileService;

            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

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
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            if (data is UIBookingModel booking)
            {
                _parking = _profileService.GetParkingById(booking.ParkingId);
                Info = _parking.Address;
                SubInfo = _parking.Cap.ToString() + " " + _parking.City;
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
                base.OnPropertyChanged("Info");
                base.OnPropertyChanged("SubInfo");
                base.OnPropertyChanged("Picture");
                base.OnPropertyChanged("PictureAspect");
                base.OnPropertyChanged("FullPrice");
                base.OnPropertyChanged("FullAvailability");

                if ((booking.StartDate == null) || (booking.StartDate < DateTime.Now))
                {
                    booking.StartDate = DateTime.Now;
                }
                if ((booking.EndDate == null) || (booking.EndDate < DateTime.Now))
                {
                    booking.EndDate = DateTime.Now;
                }
                StartDate = booking.StartDate.Date;
                StartTime = booking.StartDate.TimeOfDay;
                MinStartDate = DateTime.Now.Date;
                EndDate = booking.EndDate.Date;
                EndTime = booking.EndDate.TimeOfDay;
                MinEndDate = booking.StartDate.Date;

                // Remove seconds from times
                StartTime = StartTime.Subtract(TimeSpan.FromSeconds(StartTime.Seconds));
                EndTime = EndTime.Subtract(TimeSpan.FromSeconds(EndTime.Seconds));
                bool isFree = _parking.isFree(StartDate + StartTime, EndDate + EndTime);
                FullAvailability = (isFree) ? "Disponibile" : "Occupato";

                base.OnPropertyChanged("StartDate");
                base.OnPropertyChanged("StartTime");
                base.OnPropertyChanged("MinStartDate");
                base.OnPropertyChanged("EndDate");
                base.OnPropertyChanged("EndTime");
                base.OnPropertyChanged("MinEndDate");
            }

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
            // Check Data
            if ((StartDate + StartTime) > (EndDate + EndTime))
            {
                _dialogService.ShowAlert("Errore", "Data e ora di fine devono essere sucessive a quelle di inizio");
                return;
            }

            // Compute price
            double orderPrice = double.Parse((((EndDate + EndTime) - (StartDate + StartTime)).TotalHours * _parking.PriceMin).ToString("N2"));
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
                StartDate = StartDate+StartTime,
                EndDate = EndDate + EndTime,
                Price = orderPrice,
                UserId = int.Parse(AuthSettings.UserId),
                CarPlate = AuthSettings.User.CarPlate
            };

            // Show activity spinner
            this.IsRunning = true;
            base.OnPropertyChanged("IsRunning");

            // Ask confirm
            ConfirmStartDateTime = _order.StartDate.ToString("dd/MM/yy hh:mm");
            ConfirmEndDateTime = _order.EndDate.ToString("dd/MM/yy hh:mm");
            ConfirmPrice = _order.Price.ToString("N2") + " CHF";
            ConfirmVisible = true;

            base.OnPropertyChanged("ConfirmStartDateTime");
            base.OnPropertyChanged("ConfirmEndDateTime");
            base.OnPropertyChanged("ConfirmPrice");
            base.OnPropertyChanged("ConfirmVisible");
        }

        public async void SendOrder(OrderModel order)
        {
            try
            {
                var result = await _orderDataService.CreateOrderAsync(order);

                IsRunning = false;
                base.OnPropertyChanged("IsRunning");

                if (result != null)
                {
                    await NavigationService.NavigateToAsync<UserBookingViewModel>();
                }
                else
                {
                    await _dialogService.ShowAlert("Errore", "Impossibile eseguire l'ordine");
                }
            }
            catch (Exception e)
            {
                await _dialogService.ShowAlert("Errore", e.Message);
            }
        }

        private void OnStartDateChanged()
        {
            MinEndDate = _startDate;
            if (EndDate < _startDate)
            {
                EndDate = _startDate;
                base.OnPropertyChanged("EndDate");
            }
            base.OnPropertyChanged("MinEndDate");
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
