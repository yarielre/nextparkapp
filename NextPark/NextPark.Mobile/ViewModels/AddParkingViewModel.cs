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
        public string Address { get; set; }
        public string Cap { get; set; }
        public string City { get; set; }
        public string Notes { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public string PriceMinText { get; set; }
        public double PriceMin { get; set; }
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
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
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

            PriceMin = 1.50;
            PriceMax = 3.0;
            PriceMinText = PriceMin.ToString("N2");
            PriceMaxText = PriceMax.ToString("N2");
            PriceMaxMinimum = PriceMin;

            ParkingImage = "icon_add_photo_256.png";
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {

            if ((data != null) && (data is ParkingModel parking))
            {
                // Edit Parking
                Title = "Modifica Parcheggio";
                Address = parking.Address;
                City = parking.City;
                UID = parking.Id;
                Cap = parking.Cap.ToString();
                PriceMin = parking.PriceMin;
                PriceMax = parking.PriceMax;
                base.OnPropertyChanged("Title");
                base.OnPropertyChanged("Address");
                base.OnPropertyChanged("Cap");
                base.OnPropertyChanged("City");
                base.OnPropertyChanged("PriceMin");
                base.OnPropertyChanged("PriceMax");
                _isAuthorized = true;
                _modify = true;
                AddBtnText = "Modifica";
                base.OnPropertyChanged("AddBtnText");
                DelBtnVisible = true;
                base.OnPropertyChanged("DelBtnVisible");
            } else {
                // New Parking
                Title = "Nuovo Parcheggio";
                base.OnPropertyChanged("Title");
                _isAuthorized = false;
                _modify = false;
                AddBtnText = "Aggiungi";
                base.OnPropertyChanged("AddBtnText");
                DelBtnVisible = false;
                base.OnPropertyChanged("DelBtnVisible");
            }

            // Header
            BackText = "Parcheggi";
            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
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
        public void OnPriceMinChangedMethod(double value)
        {
            PriceMin = value;
            PriceMinText = PriceMin.ToString("N2");
            base.OnPropertyChanged("PriceMin");
            base.OnPropertyChanged("PriceMinText");
            if (PriceMax < PriceMin) {
                PriceMax = PriceMin;
                PriceMaxText = PriceMax.ToString("N2");
                base.OnPropertyChanged("PriceMax");
                base.OnPropertyChanged("PriceMaxText");
            }
            PriceMaxMinimum = PriceMin;
            base.OnPropertyChanged("PriceMaxMinimum");
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
                _dialogService.ShowAlert("Attenzione", "La foto e la posizione sono obbligatorie");
                error = true;
            }

            return error;
        }

        // Add Parking button action
        public void OnAddParkingMethod(object sender)
        {
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
                    PriceMax = PriceMax
                };

                // Start activity spinner
                IsRunning = true;
                base.OnPropertyChanged("IsRunning");

                // Send request to back-end
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

        public async void EditParkingMethod(ParkingModel model)
        {
            try
            {
                var addResponse = await _parkingDataService.Put(model, model.Id);

                if (addResponse != null)
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
                var delResponse = await _parkingDataService.Delete(parkingId);

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
                Longitude = getLocation.Longitude;
                Latitude = getLocation.Latitude;

                var result = await _geoLocatorService.GetAddressForPosition(getLocation);

                foreach (var address in result)
                {
                    Cap = address.PostalCode;
                    Address = address.FeatureName;
                    City = address.Locality;
                    base.OnPropertyChanged("Cap");
                    base.OnPropertyChanged("Address");
                    base.OnPropertyChanged("City");
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
    }
}
