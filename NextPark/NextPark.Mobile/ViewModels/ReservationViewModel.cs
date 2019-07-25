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
        private TimeSpan _startTime { get; set; }        // Reservation start time
        public TimeSpan StartTime
        {
            get { return _startTime; }
            set { _startTime = value; OnReservationTimeChanged(); }
        }
        public DateTime MinStartDate { get; set; }      // Reservation minimum start date
        private DateTime _endDate { get; set; }           // Reservation end date
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; OnReservationTimeChanged(); }
        }
        private TimeSpan _endTime { get; set; }           // Reservation end time
        public TimeSpan EndTime
        {
            get { return _endTime; }
            set { _endTime = value; OnReservationTimeChanged(); }
        }
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
            UserMoney = AuthSettings.UserCoin.ToString("N2");
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
            UserMoney = AuthSettings.UserCoin.ToString("N2");
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
                _dialogService.ShowAlert("Errore", "Data e ora di inizio devono precedere quelle di fine");
                return;
            }

            // Compute price
            double orderPrice = double.Parse((((EndDate + EndTime) - (StartDate + StartTime)).TotalHours * _parking.PriceMin).ToString("N2"));
            if (_parking.UserId == AuthSettings.User.Id)
            {
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
                Price = orderPrice,
                UserId = int.Parse(AuthSettings.UserId),
                CarPlate = AuthSettings.User.CarPlate
            };
            _order.StartDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Hours, StartTime.Minutes, 0);
            _order.EndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, EndTime.Hours, EndTime.Minutes, 0);

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

        public async void SendOrder(OrderModel order)
        {
            try
            {
                var result = await _orderDataService.CreateOrderAsync(order);

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
                    }
                    else
                    {
                        // Unexpected error
                        await _dialogService.ShowAlert("Errore", "Impossibile eseguire l'ordine");
                        await NavigationService.NavigateToAsync<HomeViewModel>();
                        return;
                    }
                }
                else
                {
                    // Unexpected error
                    await _dialogService.ShowAlert("Errore", "Impossibile eseguire l'ordine");
                    await NavigationService.NavigateToAsync<HomeViewModel>();
                    return;
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

        private void OnReservationTimeChanged()
        {
            if (_parking != null)
            {
                // Remove seconds from times
                TimeSpan startTime = StartTime.Subtract(TimeSpan.FromSeconds(StartTime.Seconds));
                TimeSpan endTime = EndTime.Subtract(TimeSpan.FromSeconds(EndTime.Seconds));

                // Update avaiability
                bool isFree = _parking.isFree(StartDate + startTime, EndDate + endTime);
                FullAvailability = (isFree) ? "Disponibile" : "Occupato";

                base.OnPropertyChanged("FullAvailability");
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
