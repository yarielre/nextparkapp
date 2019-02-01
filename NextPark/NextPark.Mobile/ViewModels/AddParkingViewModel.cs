using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using Plugin.Media;
using NextPark.Mobile.Core.Settings;
using NextPark.Mobile.Services.Data;
using NextPark.Models;

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

        public int UID { get; set; }
        public string Title { get; set; }
        public string Street { get; set; }
        public string NPA { get; set; }
        public string City { get; set; }
        public string Notes { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
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

        public bool IsRunning { get; set; }         // Activity spinner

        public ICommand OnAddParking { get; set; }
        public bool AddBtnEnabled { get; set; }
        public Color AddBtnBackgroundColor { get; set; }
        public Color AddBtnBorderColor { get; set; }

        private ImageSource _parkingImage;              // Parking image
        public ImageSource ParkingImage
        {
            get => _parkingImage;
            set => SetValue(ref _parkingImage, value);
        }
        public ICommand OnParkingImageTap { get; set; } // Parking image action

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IGeolocatorService _geoLocatorService;
        private readonly ParkingDataService _parkingDataService;

        // PRIVATE VARIABLES
        private double _minPriceValue;
        private double _maxPriceValue;
        private bool _isAuthorized;
        private bool _modify;

        // METHODS
        public AddParkingViewModel(IDialogService dialogService,
                                   IGeolocatorService geolocatorService,
                                   IApiService apiService,
                                   IAuthService authService,
                                   INavigationService navService,
                                   ParkingDataService parkingDataService)
                                   : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _geoLocatorService = geolocatorService;
            _parkingDataService = parkingDataService;

            // Header
            UserName = AuthSettings.UserName;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnParkingImageTap = new Command<object>(OnParkingImageTapMethod);
            OnAddParking = new Command<object>(OnAddParkingMethod);

            MinPriceValue = (int)1;
            MaxPriceValue = (int)1;
            base.OnPropertyChanged("MinPriceValue");
            base.OnPropertyChanged("MaxPriceValue");

            ParkingImage = "icon_add_photo_256.png";
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {

            if ((data != null) && (data is ParkingItem parking))
            {
                // Edit Parking
                Title = "Modifica Parcheggio";
                Street = parking.Address;
                City = parking.City;
                UID = parking.UID;
                base.OnPropertyChanged("Title");
                base.OnPropertyChanged("Street");
                base.OnPropertyChanged("City");
                _isAuthorized = true;
                _modify = true;
            } else {
                // New Parking
                Title = "Nuovo Parcheggio";
                base.OnPropertyChanged("Title");
                _isAuthorized = false;
                _modify = false;
            }


            // Header
            BackText = "Parcheggi";
            UserName = AuthSettings.UserName;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
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

        // Parking image tap action
        public void OnParkingImageTapMethod(object args)
        {
            TakeParkingPhoto();
        }

        // Add Parking button action
        public void OnAddParkingMethod(object sender)
        {
            // TODO: check picture and location, location is a must have!
            if (_isAuthorized) {

                // TODO: fill add parking data according to parking data model
                ParkingModel model = new ParkingModel
                {
                    Address = this.Street,
                    Cap = int.Parse(NPA),
                    City = City,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    UserId = int.Parse(AuthSettings.UserId),
                    PriceMin = _minPriceValue,
                    PriceMax = _maxPriceValue
                };

                // Start activity spinner
                IsRunning = true;
                base.OnPropertyChanged("IsRunning");

                if (_modify == true) {
                    model.Id = UID;
                    EditParkingMethod(model);
                } else {
                    AddParkingMethod(model);
                }
            }
        }

        public async void AddParkingMethod(ParkingModel model)
        {
            try {
                var addResponse = await _parkingDataService.Post(model);

                if (addResponse != null) {
                    if (addResponse is ParkingModel addedModel) {

                    }
                    await NavigationService.NavigateToAsync<UserParkingViewModel>();
                }
            } catch (Exception e) {

            } finally {
                // Stop activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
            }
        }

        public async void EditParkingMethod(ParkingModel model)
        {
            try
            {
                var addResponse = await _parkingDataService.Put(model, model.Id);

                if (addResponse != null)
                {
                    if (addResponse is ParkingModel addedModel)
                    {

                    }
                    await NavigationService.NavigateToAsync<UserParkingViewModel>();
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                // Stop activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
            }
        }

        // Take User Image
        private async void TakeParkingPhoto()
        {
            MediaFile mediaFile;
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Errore Fotocamera",
                    "Fotocamera non disponibile o non supportata.",
                    "OK");

                // TODO: remove the following line, DEMO ONLY!
                _isAuthorized = await GetCurrentLocation();

                return;
            }
            mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "parking_photo.jpg",
                PhotoSize = PhotoSize.Small
            });


            if (mediaFile == null)
                return;
            ParkingImage = ImageSource.FromStream(() => { return mediaFile.GetStream(); });

            _isAuthorized = await GetCurrentLocation();
        }

        public async Task<bool> GetCurrentLocation()
        {
            try
            {
                var permissionGaranted = await _geoLocatorService.IsPermissionGaranted();

                if (!permissionGaranted)
                {
                    // TODO: ask for location, location is a must have!
                    return false;
                }

                var getLocation = await _geoLocatorService.GetLocation();

                if (getLocation == null)
                {
                    // TODO: ask for location, location is a must have!
                    return false;
                }
                Longitude = getLocation.Latitude;
                Latitude = getLocation.Longitude;

                return true;
            }
            catch (Exception ex)
            {
                // _loggerService.LogVerboseException(ex, this).ShowVerboseException(ex, this).ThrowVerboseException(ex, this);
            }
            return false;
        }
    }
}
