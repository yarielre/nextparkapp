using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using Plugin.Media;

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

        private ImageSource _parkingImage;              // Parking image
        public ImageSource ParkingImage
        {
            get => _parkingImage;
            set => SetValue(ref _parkingImage, value);
        }
        public ICommand OnParkingImageTap { get; set; } // Parking image action

        // SERVICES
        private readonly IDialogService _dialogService;

        // PRIVATE VARIABLES
        private double _minPriceValue;
        private double _maxPriceValue;

        // METHODS
        public AddParkingViewModel(IDialogService dialogService,
                                   IApiService apiService,
                                   IAuthService authService,
                                   INavigationService navService)
                                   : base(apiService, authService, navService)
        {
            _dialogService = dialogService;

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

        // Parking image tap action
        public void OnParkingImageTapMethod(object args)
        {
            TakeParkingPhoto();
        }

        // Add Parking button action
        public void OnAddParkingMethod(object sender)
        {
            // TODO: fill add parking data according to parking data model
            // TODO: send add parking request to backend
            _dialogService.ShowAlert("Alert", "TODO: Add parking");
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
        }
    }
}
