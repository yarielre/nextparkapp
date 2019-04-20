using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using NextPark.Mobile.Controls;

using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System;
using NextPark.Mobile.Settings;
using NextPark.Mobile.UIModels;
using NextPark.Models;
using NextPark.Mobile.Services.Data;
using NextPark.Mobile.CustomControls;
using System.Collections.Generic;
using NextPark.Enums.Enums;

namespace NextPark.Mobile.ViewModels
{
    public class BookingMapParams
    {
        public CustomControls.CustomMap Map { get; set; }
        public UIBookingModel Booking { get; set; }
    }

    public class TimeSelButton
    {
        public bool isSelected { get; set; }
        public int minutes { get; set; }
    }

    public class BookingMapViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string ActionText { get; set; }      // Action button text
        public bool ActionVisible { get; set; }     // Action button visibity
        public string DeleteText { get; set; }      // Delete button text

        public CustomControls.CustomMap Map { get; set; }  // Custom Map
        public ICommand OnAction { get; set; }             // Arrived button click action
        public ICommand OnNavigate { get; set; }            // Navigate button click action
        public ICommand OnBookDel { get; set; }             // Delete button click action
        public ICommand OnRenew { get; set; }               // Renew order action
        public ICommand OnCancelRenew { get; set; }         // Cancel renew order

        public bool RenewVisible { get; set; }
        public bool RenewIsRunning { get; set; }
        public string RenewTotalTimeText { get; set; }
        public TimeSpan RenewTime { get; set; }

        // Buttons
        public Boolean Btn1IsSelected { get; set; }     // 15 min button selected
        public string Btn1SubInfo { get; set; }         // 15 min button price
        public Boolean Btn2IsSelected { get; set; }     // 45 min button selected
        public string Btn2SubInfo { get; set; }         // 45 min button price
        public Boolean Btn3IsSelected { get; set; }     // 1.0 h button selected
        public string Btn3SubInfo { get; set; }         // 1.0 h button price
        public Boolean Btn4IsSelected { get; set; }     // 2.0 h button selected
        public string Btn4SubInfo { get; set; }         // 2.0 h button price
        public Boolean Btn5IsSelected { get; set; }     // 3.0 h button selected
        public string Btn5SubInfo { get; set; }         // 3.0 h button price
        public Boolean Btn6IsSelected { get; set; }     // 4.0 h button selected
        public string Btn6SubInfo { get; set; }         // 4.0 h button price
        public ICommand OnButtonTapped { get; set; }    // Selection button tapped

        // Confirm pop-up
        public bool ConfirmVisible { get; set; }
        public string ConfirmTotalTime { get; set; }
        public string ConfirmPrice { get; set; }
        public ICommand OnConfirm { get; set; }
        public ICommand OnCancel { get; set; }

        // Terminate confirm pop-up
        public bool TerminateConfirmVisible { get; set; }
        public bool TerminateIsRunning { get; set; }
        public ICommand OnTerminateConfirm { get; set; }
        public ICommand OnTerminateCancel { get; set; }

        // PRIVATE VARIABLES
        private List<TimeSelButton> _timeSelButtons = new List<TimeSelButton>();
        private UIBookingModel order;

        // SERVICES
        private readonly IGeolocatorService _geoLocatorService;
        private readonly IDialogService _dialogService;
        private readonly IOrderDataService _orderDataService;
        private readonly IParkingDataService _parkingDataService;

        // METHODS
        public BookingMapViewModel(IGeolocatorService geolocatorService, 
                                   IDialogService dialogService,
                                   IApiService apiService, 
                                   IAuthService authService, 
                                   INavigationService navService,
                                   IParkingDataService parkingDataService,
                                   IOrderDataService orderDataService)
                                   : base(apiService, authService, navService)
        {
            _geoLocatorService = geolocatorService;
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;
            _orderDataService = orderDataService;

            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N2");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            ActionText = "";
            ActionVisible = false;
            base.OnPropertyChanged("ActionText");
            base.OnPropertyChanged("ActionVisible");
            DeleteText = "Termina";
            base.OnPropertyChanged("DeleteText");

            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnAction = new Command<object>(OnActionMethod);
            OnNavigate = new Command<object>(OnNavigateMethod);
            OnBookDel = new Command<object>(OnBookDelMethod);
            OnRenew = new Command<object>(OnRenewMethod);
            OnCancelRenew = new Command<object>(OnCancelRenewMethod);
            OnButtonTapped = new Command<string>(OnButtonTappedMethod);

            RenewVisible = false;

            ConfirmVisible = false;
            OnConfirm = new Command(OnConfirmMethod);
            OnCancel = new Command(OnCancelMethod);

            TerminateConfirmVisible = false;
            OnTerminateConfirm = new Command(OnTerminateConfirmMethod);
            OnTerminateCancel = new Command(OnTerminateCancelMethod);
        }

        public override Task InitializeAsync(object data = null)
        {
            if (data == null)
            {
                return Task.FromResult(false);
            }

            Map.MapReady += Map_MapReady;
            Map.Tapped += Map_Tapped;
            Map.PinTapped += Map_PinTapped;

            // Header
            BackText = "Indietro";
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N2");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            if (data is UIBookingModel parameter)
            {
                order = parameter;

                if (DateTime.Now < order.StartDate) {
                    if (DateTime.Now.AddMinutes(30) >= order.StartDate)
                    {
                        // Early start, max 30 minutes before
                        ActionText = "Arrivato";
                        ActionVisible = true;
                    } else {
                        ActionVisible = false;
                    }
                    base.OnPropertyChanged("ActionText");
                    base.OnPropertyChanged("ActionVisible");
                } else {
                    // Renovate
                    ActionText = "Estendi";
                    ActionVisible = true;
                    base.OnPropertyChanged("ActionText");
                    base.OnPropertyChanged("ActionVisible");
                }
                /* FUTURE IMPLEMENTATIONS
                if (DateTime.Now < order.StartDate) {
                    // Delete
                    DeleteText = "Elimina";
                    base.OnPropertyChanged("DeleteText");
                } else {
                    // Terminate
                    DeleteText = "Termina";
                    base.OnPropertyChanged("DeleteText");
                }
                */
                // Terminate
                DeleteText = "Termina";
                base.OnPropertyChanged("DeleteText");
            }

            return Task.FromResult(false);
        }

        private void Map_Tapped(object sender, CustomControls.MapTapEventArgs e)
        {
            //throw new System.NotImplementedException();
        }

        private void Map_PinTapped(object sender, CustomControls.PinTapEventArgs e)
        {
            //throw new System.NotImplementedException();
        }

        private void Map_MapReady(object sender, System.EventArgs e)
        {
            // Get parking position
            Xamarin.Forms.Maps.Position position = new Position(order.Parking.Latitude, order.Parking.Longitude);

            // Add Map Pin of parking
            var pin = new CustomPin
            {
                Id = order.Parking.Id,
                Parking = order.Parking,
                Type = PinType.Place,
                Position = position,
                Label = order.Parking.Address,
                Address = order.Parking.Cap.ToString() + " " + order.Parking.City,
                Icon = "icon_pin_green_256"
            };
            Map.Pins.Add(pin);

            // Move to parking position
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));
        }

        public override bool BackButtonPressed()
        {
            if (RenewVisible)
            {
                RenewVisible = false;
                base.OnPropertyChanged("RenewVisible");
            }
            else if (ConfirmVisible)
            {
                ConfirmVisible = false;
                RenewIsRunning = true;
                base.OnPropertyChanged("ConfirmVisible");
                base.OnPropertyChanged("RenewIsRunning");
            }
            else if (TerminateConfirmVisible) 
            {
                TerminateConfirmVisible = false;
                TerminateIsRunning = false;
                base.OnPropertyChanged("TerminateConfirmVisible");
                base.OnPropertyChanged("TerminateIsRunning");
            }
            else
            {
                OnBackClickMethod(null);
            }
            return false; // Do not propagate back button pressed
        }

        // Back Click action
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserBookingViewModel>();
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

        // Arrived button click action
        public void OnActionMethod(object sender)
        {
            if (DateTime.Now < order.StartDate)
            {
                // Early start

                // TODO: how many time before can user arrive?

                // Modify order 
                order.StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

                // Update price
                TimeSpan totalTime = order.EndDate - order.StartDate;
                order.Price = totalTime.TotalHours * order.Parking.PriceMin;
                if (order.Parking.UserId == AuthSettings.User.Id)
                {
                    order.Price = 0;
                }
                // Check user balance
                if (AuthSettings.User.Balance < order.Price)
                {
                    // Not enough credit
                    _dialogService.ShowAlert("Attenzione", "Credito insufficiente");
                    NavigationService.NavigateToAsync<MoneyViewModel>();
                    return;
                }

                EditOrder();
            }
            else
            {
                // Renew order
                RenewTime = TimeSpan.FromMinutes(0);
                RenewTotalTimeText = string.Format("{0:%h} h {0:%m} min", RenewTime);
                RenewVisible = true;
                base.OnPropertyChanged("RenewVisible");
                base.OnPropertyChanged("RenewTotalTimeText");

                // Init time selection buttons
                _timeSelButtons.Clear();
                _timeSelButtons.Add(new TimeSelButton { isSelected = false, minutes = 15 });    // 15 min
                _timeSelButtons.Add(new TimeSelButton { isSelected = false, minutes = 30 });    // 30 min
                _timeSelButtons.Add(new TimeSelButton { isSelected = false, minutes = 60 });    // 1 h
                _timeSelButtons.Add(new TimeSelButton { isSelected = false, minutes = 120 });   // 2 h
                _timeSelButtons.Add(new TimeSelButton { isSelected = false, minutes = 180 });   // 3 h
                _timeSelButtons.Add(new TimeSelButton { isSelected = false, minutes = 240 });   // 4 h

                UpdateButtonStatus();
            }

        }

        // Navigate button click action
        public void OnNavigateMethod(object sender)
        {
            // Get parking location
            var location = new Xamarin.Essentials.Location(order.Parking.Latitude, order.Parking.Longitude);
            // Set navigation options
            var options = new Xamarin.Essentials.MapLaunchOptions { NavigationMode = Xamarin.Essentials.NavigationMode.Driving };
            // Call native map application
            Xamarin.Essentials.Map.OpenAsync(location, options);
        }

        // Delete button click action
        public void OnBookDelMethod(object sender)
        {
            /* FUTURE IMPLEMENTATION 
            if (DateTime.Now < order.StartDate)
            {
                // Delete
                // TODO: manage booking delete action
                DeleteOrder();
            }
            else
            {
                // Terminate
                TerminateOrder();
            }
            */
            // Ask terminate confirmation
            TerminateConfirmVisible = true;
            base.OnPropertyChanged("TerminateConfirmVisible");
        }

        public async void EditOrder()
        {
            try
            {
                var result = await _orderDataService.EditOrderAsync(order.Id, order);
                // TODO: Check edit result

                // TODO: Show Estendi if sucessfull

            }
            catch (Exception e)
            {
                await _dialogService.ShowAlert("Errore", e.Message);
            }
        }

        public async void DeleteOrder()
        {
            try
            {
                var result = await _orderDataService.DeleteOrdersAsync(order.Id);

            }
            catch (Exception e)
            {
                await _dialogService.ShowAlert("Errore", e.Message);
            }
        }

        public async void TerminateOrder()
        {
            try {
                var result = await _orderDataService.TerminateOrderAsync(order.Id);

                TerminateIsRunning = false;
                TerminateConfirmVisible = false;
                base.OnPropertyChanged("TerminateIsRunning");
                base.OnPropertyChanged("TerminateConfirmVisible");

                // Check terminate order result
                if (result != null) {

                }
                await NavigationService.NavigateToAsync<UserBookingViewModel>();

            } catch (Exception e) {
                await _dialogService.ShowAlert("Errore", e.Message);
            }
        }

        // Renew order button click action
        public void OnRenewMethod(object sender)
        {
            double price = (RenewTime.TotalHours * order.Parking.PriceMin);
            if (order.Parking.UserId == AuthSettings.User.Id)
            {
                price = 0.0;
            }

            // Ask confirm
            ConfirmTotalTime = string.Format("{0:%h} h {0:%m} min", RenewTime);
            ConfirmPrice = (price).ToString("N2") + " CHF";
            ConfirmVisible = true;
            RenewVisible = false;

            base.OnPropertyChanged("ConfirmTotalTime");
            base.OnPropertyChanged("ConfirmPrice");
            base.OnPropertyChanged("ConfirmVisible");
            base.OnPropertyChanged("RenewVisible");
        }

        public async void RenewOrder()
        {
            try {
                if (RenewTime.TotalMinutes > 0)
                {
                    // Update order time
                    order.EndDate = order.EndDate.AddMinutes(RenewTime.TotalMinutes);

                    // Update order price
                    TimeSpan totalTime = order.EndDate - order.StartDate;
                    order.Price = totalTime.TotalHours * order.Parking.PriceMin;
                    if (order.UserId == AuthSettings.User.Id)
                    {
                        order.Price = 0;
                    }

                    // Check user balance
                    if (AuthSettings.User.Balance < order.Price)
                    {
                        // Not enough credit
                        await _dialogService.ShowAlert("Attenzione", "Credito insufficiente");
                        await NavigationService.NavigateToAsync<MoneyViewModel>();
                        return;
                    }

                    // Send order update
                    //var result = await _orderDataService.EditOrderAsync(order.Id, order);
                    var result = await _orderDataService.RenovateOrderAsync(order.Id, order);
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
                            return;
                        }
                        else
                        {
                            // Unexpected error
                            await _dialogService.ShowAlert("Errore", "Impossibile rinnovare l'ordine");
                            return;
                        }
                    }
                    else
                    {
                        // Unexpected error
                        await _dialogService.ShowAlert("Errore", "Impossibile rinnovare l'ordine");
                        return;
                    }
                }
            } catch (Exception e) {
                await _dialogService.ShowAlert("Errore", e.Message);
            } finally {
                RenewIsRunning = false;
                ConfirmVisible = false;
                base.OnPropertyChanged("RenewIsRunning");
                base.OnPropertyChanged("ConfirmVisible");
            }
        }

        public void OnCancelRenewMethod(object sender)
        {
            RenewVisible = false;
            base.OnPropertyChanged("RenewVisible");
        }

        // Selection Button Tapped action
        public void OnButtonTappedMethod(string identifier)
        {
            UInt16 btnIndex;

            // Get the tapped selection button
            btnIndex = Convert.ToUInt16(identifier);
            if (btnIndex > 0) btnIndex--;
            if (_timeSelButtons[btnIndex].isSelected) {
                // If already selected, deselect it and remove it from total
                _timeSelButtons[btnIndex].isSelected = false;
                if (RenewTime >= TimeSpan.FromMinutes(_timeSelButtons[btnIndex].minutes)) {
                    RenewTime -= TimeSpan.FromMinutes(_timeSelButtons[btnIndex].minutes);
                }
            } else {
                // Select the button and add its time to total
                _timeSelButtons[btnIndex].isSelected = true;
                RenewTime += TimeSpan.FromMinutes(_timeSelButtons[btnIndex].minutes);
            }

            // Update renew time
            RenewTotalTimeText = string.Format("{0:%h} h {0:%m} min", RenewTime);
            base.OnPropertyChanged("RenewTotalTimeText");

            UpdateButtonStatus();
        }

        public void UpdateButtonStatus()
        {
            // Update Buttons
            Btn1IsSelected = _timeSelButtons[0].isSelected;
            Btn2IsSelected = _timeSelButtons[1].isSelected;
            Btn3IsSelected = _timeSelButtons[2].isSelected;
            Btn4IsSelected = _timeSelButtons[3].isSelected;
            Btn5IsSelected = _timeSelButtons[4].isSelected;
            Btn6IsSelected = _timeSelButtons[5].isSelected;
            base.OnPropertyChanged("Btn1IsSelected");
            base.OnPropertyChanged("Btn2IsSelected");
            base.OnPropertyChanged("Btn3IsSelected");
            base.OnPropertyChanged("Btn4IsSelected");
            base.OnPropertyChanged("Btn5IsSelected");
            base.OnPropertyChanged("Btn6IsSelected");
            base.OnPropertyChanged("Btn7IsSelected");
            base.OnPropertyChanged("Btn8IsSelected");
        }

        public void OnConfirmMethod()
        {
            RenewIsRunning = true;
            base.OnPropertyChanged("RenewIsRunning");
            RenewOrder();
        }

        public void OnCancelMethod()
        {
            ConfirmVisible = false;
            base.OnPropertyChanged("ConfirmVisible");
            // Hide activity spinner
            RenewIsRunning = false;
            base.OnPropertyChanged("RenewIsRunning");
        }

        public void OnTerminateConfirmMethod()
        {
            // Show activity spinner
            TerminateIsRunning = true;
            base.OnPropertyChanged("TerminateIsRunning");
            TerminateOrder();
        }

        public void OnTerminateCancelMethod()
        {
            TerminateIsRunning = false;
            TerminateConfirmVisible = false;
            base.OnPropertyChanged("TerminateConfirmVisible");
            base.OnPropertyChanged("TerminateIsRunning");
        }
    }
}
