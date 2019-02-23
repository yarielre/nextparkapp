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
using NextPark.Mobile.Services.DataInterface;

namespace NextPark.Mobile.ViewModels
{
    public class BookingMapParams
    {
        public CustomControls.CustomMap Map { get; set; }
        public UIBookingModel Booking { get; set; }
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

        private UIBookingModel order;
        private ParkingModel parking;

        // SERVICES
        private readonly IGeolocatorService _geoLocatorService;
        private readonly IDialogService _dialogService;
        private readonly IParkingDataService _parkingDataService;

        // METHODS
        public BookingMapViewModel(IGeolocatorService geolocatorService, 
                                   IDialogService dialogService,
                                   IApiService apiService, 
                                   IAuthService authService, 
                                   INavigationService navService,
                                   IParkingDataService parkingDataService)
                                   : base(apiService, authService, navService)
        {
            _geoLocatorService = geolocatorService;
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;

            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            ActionText = "";
            ActionVisible = false;
            base.OnPropertyChanged("ActionText");
            base.OnPropertyChanged("ActionVisible");
            DeleteText = "Elimina";
            base.OnPropertyChanged("DeleteText");

            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnAction = new Command<object>(OnActionMethod);
            OnNavigate = new Command<object>(OnNavigateMethod);
            OnBookDel = new Command<object>(OnBookDelMethod);
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
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            if (data is UIBookingModel parameter)
            {
                order = parameter;

                if (DateTime.Now < order.StartDate) {
                    // Early start
                    ActionText = "Arrivato";
                    ActionVisible = true;
                    base.OnPropertyChanged("ActionText");
                    base.OnPropertyChanged("ActionVisible");
                } else {
                    // Renovate
                    ActionText = "Aggiorna";
                    ActionVisible = true;
                    base.OnPropertyChanged("ActionText");
                    base.OnPropertyChanged("ActionVisible");
                }

                if (DateTime.Now < order.StartDate) {
                    // Delete
                    DeleteText = "Elimina";
                    base.OnPropertyChanged("DeleteText");
                } else {
                    // Terminate
                    DeleteText = "Termina";
                    base.OnPropertyChanged("DeleteText");
                }
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
                Id = parking.Id,
                Parking = parking,
                Type = PinType.Place,
                Position = position,
                Label = parking.Address,
                Address = parking.Cap.ToString() + " " + parking.City,
                Icon = "icon_pin_green_256"
            };
            Map.Pins.Add(pin);

            // Move to parking position
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));
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
                // TODO: manage user arrived
                _dialogService.ShowAlert("Alert", "TODO: manage user arrived");
            }
            else
            {
                // Renovate
                // TODO: manage user arrived
                _dialogService.ShowAlert("Alert", "TODO: manage renovate order");
            }

        }

        // Navigate button click action
        public void OnNavigateMethod(object sender)
        {
            // TODO: manage navigation to parking
            _dialogService.ShowAlert("Alert", "TODO: manage navigation to parking");
        }

        // Delete button click action
        public void OnBookDelMethod(object sender)
        {
            if (DateTime.Now < order.StartDate)
            {
                // Delete
                // TODO: manage booking delete action
                _dialogService.ShowAlert("Alert", "TODO: manage booking delete action");
            }
            else
            {
                // Terminate
                // TODO: manage booking delete action
                _dialogService.ShowAlert("Alert", "TODO: manage terminate oreder");
            }
        }
    }
}
