using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using Plugin.Media;
using NextPark.Mobile.Settings;
using NextPark.Mobile.Services.Data;
using NextPark.Models;
using Plugin.Geolocator.Abstractions;

namespace NextPark.Mobile.ViewModels
{
    public class AddParkingViewModel : BaseViewModel
    {
        private const double PRICE_MIN = 1.50;
        private const double PRICE_MAX = 10.00;

        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header username
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public int UID { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Cap { get; set; }
        public string City { get; set; }
        public string Notes { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public string PriceMinText { get; set; }
        public double PriceMin { get; set; }
        public Color PriceMinDownBorderColor { get; set; }
        public bool PriceMinDownEnable { get; set; }
        public ICommand OnPriceMinDown { get; set; }
        public Color PriceMinUpBorderColor { get; set; }
        public bool PriceMinUpEnable { get; set; }
        public ICommand OnPriceMinUp { get; set; }

        public string PriceMaxText { get; set; }
        public double PriceMax { get; set; }
        public double PriceMaxMinimum { get; set; }

        public bool IsRunning { get; set; }         // Activity spinner

        public ICommand OnAddParking { get; set; }
        public string AddBtnText { get; set; }

        public ICommand OnDelParking { get; set; }
        public bool DelBtnVisible { get; set; }

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
        private readonly IParkingDataService _parkingDataService;

        // PRIVATE VARIABLES
        private bool _isAuthorized;
        private bool _addressUpdated;
        private bool _editing;
        private MediaFile mediaFile;
        private ParkingModel _parking;

        private string _autoAddress { get; set; }
        private string _autoCap { get; set; }
        private string _autoCity { get; set; }
        private double _autoLatitude { get; set; }
        private double _autoLongitude { get; set; }

        // METHODS
        public AddParkingViewModel(IDialogService dialogService,
                                   IGeolocatorService geolocatorService,
                                   IApiService apiService,
                                   IAuthService authService,
                                   INavigationService navService,
                                   IParkingDataService parkingDataService)
                                   : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _geoLocatorService = geolocatorService;
            _parkingDataService = parkingDataService;

            // Header
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N2");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            //
            OnParkingImageTap = new Command<object>(OnParkingImageTapMethod);
            OnAddParking = new Command<object>(OnAddParkingMethod);
            OnDelParking = new Command<object>(OnDelParkingMethod);
            OnPriceMinDown = new Command(OnPriceMinDownMethod);
            OnPriceMinUp = new Command(OnPriceMinUpMethod);

            // auto address init
            _autoAddress = "";
            _autoCap = "";
            _autoCity = "";
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {

            if ((data != null) && (data is ParkingModel parking))
            {
                // Edit Parking
                _parking = parking;
                Title = "Modifica Parcheggio";
                Address = parking.Address;
                City = parking.City;
                UID = parking.Id;
                Cap = parking.Cap.ToString();
                OnPriceMinChangedMethod(parking.PriceMin);
                PriceMax = parking.PriceMax;
                if (string.IsNullOrEmpty(parking.ImageUrl))
                {
                    ParkingImage = "icon_no_photo.png";
                }
                else
                {
                    ParkingImage = ApiSettings.BaseUrl + parking.ImageUrl;
                }

                base.OnPropertyChanged("Title");
                base.OnPropertyChanged("Address");
                base.OnPropertyChanged("Cap");
                base.OnPropertyChanged("City");

                _isAuthorized = true;
                _editing = true;
                AddBtnText = "Modifica";
                base.OnPropertyChanged("AddBtnText");
                DelBtnVisible = true;
                base.OnPropertyChanged("DelBtnVisible");
            } else {
                // New Parking
                Title = "Nuovo Parcheggio";
                base.OnPropertyChanged("Title");
                ParkingImage = "icon_add_photo_256.png";
                _isAuthorized = false;
                _editing = false;
                AddBtnText = "Aggiungi";
                base.OnPropertyChanged("AddBtnText");
                DelBtnVisible = false;
                base.OnPropertyChanged("DelBtnVisible");
                OnPriceMinChangedMethod(1.50);
            }

            // Header
            BackText = "Parcheggi";
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N2");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            return Task.FromResult(false);
        }

        // Notify Propery Changed
        private void NotifyPropertyChanged(string v)
        {
            base.OnPropertyChanged(v);
        }

        public override bool BackButtonPressed()
        {
            OnBackClickMethod(null);
            return false; // Do not propagate back button pressed
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

        public void OnPriceMinDownMethod()
        {
            PriceMin = PriceMin - 0.5;
            if (PriceMin < PRICE_MIN)
            {
                PriceMin = PRICE_MIN;
            }
            OnPriceMinChangedMethod(PriceMin);
        }

        public void OnPriceMinUpMethod()
        {
            PriceMin = PriceMin + 0.5;
            if (PriceMin > PRICE_MAX)
            {
                PriceMin = PRICE_MAX;
            }
            OnPriceMinChangedMethod(PriceMin);
        }

        // On Minimum price value changed action
        public void OnPriceMinChangedMethod(double value)
        {
            PriceMin = value;
            PriceMinText = PriceMin.ToString("N2");
            if (PriceMin <= PRICE_MIN) {
                // Disable PriceMinDown button
                PriceMinDownBorderColor = Color.LightGray;
                PriceMinDownEnable = false;
            } else {
                // Enable PriceMinDown button
                PriceMinDownBorderColor = (Color)Application.Current.Resources["NextParkColor1"];
                PriceMinDownEnable = true;
            }
            if (PriceMin >= PRICE_MAX) {
                // Disable PriceMinUp button
                PriceMinUpBorderColor = Color.LightGray;
                PriceMinUpEnable = false;
            } else {
                // Enable PriceMinUp button
                PriceMinUpBorderColor = (Color)Application.Current.Resources["NextParkColor1"];
                PriceMinUpEnable = true;
            }
            base.OnPropertyChanged("PriceMin");
            base.OnPropertyChanged("PriceMinText");
            base.OnPropertyChanged("PriceMinDownBorderColor");
            base.OnPropertyChanged("PriceMinDownEnable");
            base.OnPropertyChanged("PriceMinUpBorderColor");
            base.OnPropertyChanged("PriceMinUpEnable");

            /* FUTURE FEATURE
            if (PriceMax < PriceMin) {
                PriceMax = PriceMin;
                PriceMaxText = PriceMax.ToString("N2");
                base.OnPropertyChanged("PriceMax");
                base.OnPropertyChanged("PriceMaxText");
            }
            PriceMaxMinimum = PriceMin;
            base.OnPropertyChanged("PriceMaxMinimum");
            */
        }

        // On Maximum price value changed action
        public void OnPriceMaxChangedMethod(double value)
        {
            if (value < PriceMin)
            {
                PriceMax = PriceMin;
                PriceMaxText = PriceMax.ToString("N2");
                base.OnPropertyChanged("PriceMax");
                base.OnPropertyChanged("PriceMaxText");
            } else {
                PriceMax = value;
                PriceMaxText = PriceMax.ToString("N2");
                base.OnPropertyChanged("PriceMax");
                base.OnPropertyChanged("PriceMaxText");
            }
        }

        // Parking image tap action
        public void OnParkingImageTapMethod(object args)
        {
            TakeParkingPhoto();
        }

        private bool AddParkingDataCheck()
        {
            bool error = false;

            // Address
            if (!error && string.IsNullOrEmpty(Address))
            {
                _dialogService.ShowAlert("Errore Indirizzo", "Inserire una via valida");
                error = true;
            }
            // Cap
            if (!error && string.IsNullOrEmpty(Cap))
            {
                _dialogService.ShowAlert("Errore Indirizzo", "Inserire un codice postale valido");
                error = true;
            }
            // City
            if (!error && string.IsNullOrEmpty(City))
            {
                _dialogService.ShowAlert("Errore Indirizzo", "Inserire un comune valido");
                error = true;
            }
            // Position
            if (!error && !_isAuthorized) {
                _dialogService.ShowAlert("Attenzione", "Non è stata rilevata una posizione valida. Cancellare l'indirizzo e scattare una foto al parcheggio.");
                error = true;
            }

            return error;
        }

        // Add Parking button action
        public void OnAddParkingMethod(object sender)
        {
            OnAddParkingAsync();
        }

        public async void OnAddParkingAsync()
        {
            // Check if address has been changed by user
            if (!_autoAddress.Equals(Address) || !_autoCap.Equals(Cap) || !_autoCity.Equals(City))
            {
                // use user data to get position
                _isAuthorized = await GetLocationByAddress();
            } else {
                // use camera position otherwise
                Latitude = _autoLatitude;
                Longitude = _autoLongitude;
            }

            // Check input data
            if (!AddParkingDataCheck()) {

                // Create model 
                ParkingModel model = new ParkingModel
                {
                    Address = Address,
                    Cap = (Cap != null) ? int.Parse(Cap) : 0,
                    City = City,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    UserId = AuthSettings.User.Id,
                    PriceMin = PriceMin,
                    PriceMax = PriceMax,
                    State = "CH",
                    Status = Enums.Enums.ParkingStatus.Enabled,
                    //ImageBinary = memoryStream.ToArray()
                };

                // Image converter
                if (mediaFile != null)
                {
                    var memoryStream = new System.IO.MemoryStream();
                    mediaFile.GetStream().CopyTo(memoryStream);
                    model.ImageBinary = memoryStream.ToArray();
                    model.ImageUrl = null;
                } else if (_parking != null) {
                    model.ImageBinary = null;
                    model.ImageUrl = _parking.ImageUrl;
                }

                // Start activity spinner
                IsRunning = true;
                base.OnPropertyChanged("IsRunning");

                // Send request to back-end
                if (_editing == true) {
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
                var addResponse = await _parkingDataService.CreateParkingAsync(model);

                if (addResponse != null) {
                    await NavigationService.NavigateToAsync<UserParkingViewModel>();
                }
            } 
            catch (Exception e)
            {
                await _dialogService.ShowAlert("Errore", e.Message);
            } 
            finally
            {
                // Stop activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
            }
        }

        public async void EditParkingMethod(ParkingModel parking)
        {
            try
            {
                var result = await _parkingDataService.EditParkingAsync(parking);

                if (result != null)
                {
                    await NavigationService.NavigateToAsync<UserParkingViewModel>();
                }
            }
            catch (Exception e)
            {
                await _dialogService.ShowAlert("Errore", e.Message);
            }
            finally
            {
                // Stop activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
            }
        }

        // Delete Parking button action
        public void OnDelParkingMethod(object sender)
        {
            // TODO: check picture and location, location is a must have!
            if (_isAuthorized)
            {
                // Start activity spinner
                IsRunning = true;
                base.OnPropertyChanged("IsRunning");

                DeleteParkingMethod(UID);
            }
        }

        public async void DeleteParkingMethod(int parkingId)
        {
            try
            {
                var delResponse = await _parkingDataService.DeleteParkingsAsync(parkingId);

                if (delResponse != null)
                {
                    await NavigationService.NavigateToAsync<UserParkingViewModel>();
                }
            }
            catch (Exception e)
            {
                await _dialogService.ShowAlert("Errore", e.Message);
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

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Errore Fotocamera",
                    "Fotocamera non disponibile o non supportata.",
                    "OK");

                // TODO: remove the following line, DEMO ONLY!
                mediaFile = await CrossMedia.Current.PickPhotoAsync();
                if (mediaFile == null)
                    return;
                ParkingImage = ImageSource.FromStream(() => { return mediaFile.GetStream(); });

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

                _autoLatitude = getLocation.Latitude;
                _autoLongitude = getLocation.Longitude;

                var result = await _geoLocatorService.GetAddressForPosition(getLocation);

                foreach (var address in result)
                {
                    _autoCap = address.PostalCode;
                    _autoAddress = address.Thoroughfare + " " + address.SubThoroughfare;
                    _autoCity = address.Locality;

                    if (string.IsNullOrEmpty(Address))
                    {
                        Cap = _autoCap;
                        Address = _autoAddress;
                        City = _autoCity;
                        base.OnPropertyChanged("Cap");
                        base.OnPropertyChanged("Address");
                        base.OnPropertyChanged("City");
                    }
                    break;
                }
                return true;
            }
            catch (Exception ex)
            {
                // _loggerService.LogVerboseException(ex, this).ShowVerboseException(ex, this).ThrowVerboseException(ex, this);
            }
            return false;
        }

        public async Task<bool> GetLocationByAddress()
        {
            try {
                string SearchText = Address + " " + Cap + " " + City;
                var result = await _geoLocatorService.GetPositionForAddress(SearchText);
                if (result == null)
                {
                    // no results found
                    return false;
                }

                foreach (var location in result)
                {
                    Longitude = location.Longitude;
                    Latitude = location.Latitude;
                    return true;
                }
                // no results found
                return false;

            } catch (Exception e) {
                return false;
            }
        }
    }
}
