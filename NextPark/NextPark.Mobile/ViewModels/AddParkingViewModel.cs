using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using NextPark.Models;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using NextPark.Mobile.Services.Data;

namespace NextPark.Mobile.ViewModels
{
    public class AddParkingViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header username
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string Street { get; set; }
        public string NPA { get; set; }
        public string City { get; set; }
        public string Notes { get; set; }
        public double MinPriceValue 
        {
            get { return this._minPriceValue; }
            set {
                this._minPriceValue = value;
                NotifyPropertyChanged("MinPriceValue");
                OnMinPriceChangedMethod(value); 
            }
        }

        public ICommand OnMinPriceChanged { get; set; }
        public string MinPriceText { get; set; }
        public double MaxPriceValue
        {
            get { return this._maxPriceValue; }
            set
            {
                this._maxPriceValue = value;
                NotifyPropertyChanged("MaxPriceValue");
                OnMaxPriceChangedMethod(value);
            }
        }
        public ICommand OnMaxPriceChanged { get; set; }
        public string MaxPriceText { get; set; }
        public double MaxPriceLowerBound { get; set; }

        public ICommand OnAddParking { get; set; }
        public bool AddBtnEnabled { get; set; }
        public Color AddBtnBackgroundColor { get; set; }
        public Color AddBtnBorderColor { get; set; }

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IGeolocatorService _geoLocatorService;

        // PRIVATE VARIABLES
        private double _minPriceValue;
        private double _maxPriceValue;

        // METHODS
        public AddParkingViewModel(IDialogService dialogService,
                                   IGeolocatorService geoLocatorService,
                                   IApiService apiService,
                                   IAuthService authService,
                                   INavigationService navService)
                                   : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _geoLocatorService = geoLocatorService;

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            OnAddParking = new Command<object>(OnAddParkingMethod);

            MinPriceValue = (int)1;
            MaxPriceValue = (int)1;
            base.OnPropertyChanged("MinPriceValue");
            base.OnPropertyChanged("MaxPriceValue");
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            /*
            if (data != null)
            {
                return Task.FromResult(false);
            }
            */

            // Header
            BackText = "Parcheggi";
            UserName = "Jonny";
            UserMoney = "8";
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            MinPriceText = String.Format("Prezzo minimo: {0} CHF", (int)MinPriceValue);
            MaxPriceText = String.Format("Prezzo massimo: {0} CHF", (int)MaxPriceValue);
            AddBtnBackgroundColor = Color.FromHex("#E3E3E3");
            AddBtnBorderColor = Color.Gray;
            base.OnPropertyChanged("MinPriceText");
            base.OnPropertyChanged("MaxPriceText");
            base.OnPropertyChanged("AddBtnBackgroundColor");
            base.OnPropertyChanged("AddBtnBorderColor");

            return Task.FromResult(false);
        }

        // Notify Propery Changed
        private void NotifyPropertyChanged(string v)
        {
            base.OnPropertyChanged(v);
        }

        // Back Click action
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserParkingViewModel>();
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

        // On Minimum price value changed action
        public void OnMinPriceChangedMethod(double value)
        {
            MinPriceText = String.Format("Prezzo minimo: {0} CHF", (int)value);
            MaxPriceLowerBound = value;
            base.OnPropertyChanged("MinPriceText");
            base.OnPropertyChanged("MaxPriceLowerBound");
            EnableAddButton();
        }

        // On Maximum price value changed action
        public void OnMaxPriceChangedMethod(double value)
        {
            MaxPriceText = String.Format("Prezzo massimo: {0} CHF", (int)value);
            base.OnPropertyChanged("MaxPriceText");
            EnableAddButton();
        }

        // Enable Add Parking button
        public void EnableAddButton()
        {
            AddBtnEnabled = true;
            AddBtnBackgroundColor = Color.FromHex("#8CC63F");
            AddBtnBorderColor = Color.FromHex("#8CC63F");
            base.OnPropertyChanged("AddBtnEnabled");
            base.OnPropertyChanged("AddBtnBackgroundColor");
            base.OnPropertyChanged("AddBtnBorderColor");
        }

        // Add Parking button action
        public void OnAddParkingMethod(object sender)
        {


            // TODO: send add parking request to backend
            AddNewParking();
            /* var eventResponse = await DataService.GetInstance().AddParkingEvent(ParkingEvent);

            if (eventResponse.IsSuccess)
            {
                ParkingEvent = eventResponse.Result as EventModel; //TODO: Use a generic response to avoid this CAST!
                IsRunning = false;
            }
            */
            _dialogService.ShowAlert("Alert", "TODO: Add parking");
        }

        public async void AddNewParking()
        {
            ParkingDataService parkingDataService = new ParkingDataService((ApiService)base.ApiService);

            // TODO: fill add parking data according to parking data model
            ParkingModel parkingModel = new ParkingModel();

            // TODO: get position on parking picture capture
            // Get Position
            Position position = new Position(0, 0);
            try
            {
                var permissionGaranted = await _geoLocatorService.IsPermissionGaranted();

                if (!permissionGaranted) return;

                var getLocation = await _geoLocatorService.GetLocation();

                parkingModel.Latitude = getLocation.Latitude.ToString();
                parkingModel.Longitude = getLocation.Longitude.ToString();
                parkingModel.UserId = 1;

                var response = await parkingDataService.Post(parkingModel);
                await _dialogService.ShowAlert("Alert", response.ToString());
            }
            catch (Exception ex)
            {
                // _loggerService.LogVerboseException(ex, this).ShowVerboseException(ex, this).ThrowVerboseException(ex, this);
            }
        }
    }
}
