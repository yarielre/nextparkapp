using GalaSoft.MvvmLight.Command;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Services;
using NextPark.Models;
using Plugin.Geolocator.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using Position = Xamarin.Forms.Maps.Position;

namespace Inside.Xamarin.ViewModels
{
    public class ParkingCreateViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private ObservableCollection<ParkingCategoryModel> _categories;
        private bool _isRunning;
        private bool _isEnabled;
        private string _iconNameBasedOnCategory;
        private MediaFile _mediaFile;
        private ImageSource _parkingPhoto;
        private ObservableCollection<ParkingTypeModel> _parkingTypes;
        private ParkingCategoryModel _selectedCategory;
        private ParkingTypeModel _selectedParkingType;
        private double _parkingPrice;

        public ParkingCreateViewModel()
        {

        }

        public ParkingCreateViewModel(Position position)
        {
            _iconNameBasedOnCategory = "ic_location_black";
            Coordenate = position;
            GetParkingCategoriesAsync();
            GetParkingTypesAsync();
            ParkingPhoto = "add_photo";
            _navigationService = NavigationService.GetInstance();
            MessagingCenter.Subscribe<EventModel>(this, Messages.ParkingEventCreated,
                parkingEvent => { ParkingEvent = parkingEvent; });
        }


        public Position Coordenate { get; set; }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetValue(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }
        public double ParkingPrice
        {
            get => _parkingPrice;
            set => SetValue(ref _parkingPrice, value);
        }
        public ImageSource ParkingPhoto
        {
            get => _parkingPhoto;
            set => SetValue(ref _parkingPhoto, value);
        }

        public EventModel ParkingEvent { get; set; }

        public string IconNameBasedOnCategory
        {
            get => _iconNameBasedOnCategory;
            set => SetValue(ref _iconNameBasedOnCategory, value);
        }

        public Address Address { get; set; }

        public ObservableCollection<ParkingCategoryModel> Categories
        {
            get => _categories;
            set => SetValue(ref _categories, value);
        }

        public ParkingCategoryModel SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetValue(ref _selectedCategory, value);
                GetColorParkingIcon();
                GetParkingPrice();
            }
        }

        public ObservableCollection<ParkingTypeModel> ParkingTypes
        {
            get => _parkingTypes;
            set => SetValue(ref _parkingTypes, value);
        }

        public ParkingTypeModel SelectedParkingType
        {
            get => _selectedParkingType;
            set
            {
                SetValue(ref _selectedParkingType, value);
                GetParkingPrice();
            }
        }

        public ICommand ParkingCreateCommand => new RelayCommand(ParkingCreate);
        public ICommand AddPhotoCommand => new RelayCommand(AddPhoto);
        public ICommand EventCommand => new RelayCommand(CreateParkingEvent);


        private async void AddPhoto()
        {
            await CrossMedia.Current.Initialize();
            var source = await Application.Current.MainPage.DisplayActionSheet(
                Languages.ParkingCreateAddPhotoHeaderAlert,
                Languages.GeneralCancel,
                null,
                Languages.ParkingCreateAddPhotoOption1,
                Languages.ParkingCreateAddPhotoOption2);
            if (source == null)
            {
                _mediaFile = null;
                return;
            }
            if (source == Languages.ParkingCreateAddPhotoOption2)
                TakeParkingPhoto();
            if (source == Languages.ParkingCreateAddPhotoOption1)
                PickParkingPhoto();
        }

        private async void TakeParkingPhoto()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.ParkingCreateTakePhotoHeaderAlert,
                    Languages.ParkingCreateTakePhotoAlert,
                    Languages.GeneralAccept);
                return;
            }
            _mediaFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "parking_photo.jpg",
                PhotoSize = PhotoSize.Small
            });


            if (_mediaFile == null)
                return;
            ParkingPhoto = ImageSource.FromStream(() => { return _mediaFile.GetStream(); });
        }

        private async void PickParkingPhoto()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.ParkingCreatePickPhotoHeaderAlert,
                    Languages.ParkingCreatePickPhotoAlert,
                    Languages.GeneralAccept);
                return;
            }
            _mediaFile = await CrossMedia.Current.PickPhotoAsync();
            if (_mediaFile == null)
                return;
            ParkingPhoto = ImageSource.FromStream(() => { return _mediaFile.GetStream(); });
        }

        private async void ParkingCreate()
        {
            if (ParkingEvent == null)
            {
                 DialogService.GetInstance().ShowInfoAlertOnMaster(
                    "Alert",
                    "You must set the parking availability");
                return;
            }
            if (SelectedCategory == null)
            {
                await DialogService.GetInstance().ShowDialogAlertOnMaster(
                    "Alert",
                    "You must set the parking Category");
                return;
            }
            if (SelectedParkingType == null)
            {
                await DialogService.GetInstance().ShowDialogAlertOnMaster(
                    "Alert",
                    "You must set the parking type");
                return;
            }
            IsRunning = true;
            var eventResponse = await DataService.GetInstance().AddParkingEvent(ParkingEvent);

            if (eventResponse.IsSuccess)
            {
                ParkingEvent = eventResponse.Result as EventModel; //TODO: Use a generic response to avoid this CAST!
                IsRunning = false;
            }
            else
            {
                await DialogService.GetInstance().ShowDialogAlertOnMaster(
                    "Error",
                    "Error trying create parking event.");
                IsRunning = false;
                return;

            }

            try
            {
                byte[] imageArray = null;
                if (_mediaFile != null)
                {
                    imageArray = FilesHelper.ReadFully(_mediaFile.GetStream());
                    _mediaFile.Dispose();
                }

                var parking = new ParkingModel
                {
                    Latitude = Coordenate.Latitude.ToString(),
                    Longitude = Coordenate.Longitude.ToString(),
                    ParkingCategoryId = SelectedCategory.Id,
                    ParkingTypeId = SelectedParkingType.Id,
                    ParkingEventId = ParkingEvent.Id,
                    UserId = MainViewModel.GetInstance().CurrentUser.Id,
                    ImageBinary = imageArray
                };

                IsRunning = true;

                var parkingResponse = await DataService.GetInstance().AddParking(parking);
              
                if (!parkingResponse.IsSuccess)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        Languages.GeneralError,
                        Languages.ParkingCreateAddAlert,
                        Languages.GeneralAccept);
                    ParkingPhoto = "add_photo";
                    IsRunning = false;
                    await _navigationService.BackOnMaster();
                    return;
                }

                MessagingCenter.Send(parkingResponse.Result as ParkingModel, Messages.NewParkingCreated);
                IsRunning = false;
                await _navigationService.BackOnMaster();
            }
            catch (Exception e)
            {
                // TODO Esto luego hay que ver que será mejor en caso de capturar la exepcion 
                if (e is NullReferenceException)
                {
                  await  DialogService.GetInstance().ShowDialogAlertOnMaster(
                        "Null error",
                        e.Message);
                    IsRunning = false;
                }
                await NavigationService.GetInstance().BackOnMaster();
            }

           
        }

        private async void GetParkingTypesAsync()
        {
            //APIService se puede utilizar directamente si del backend llega ya el Model y no la entidad.
            //Cargar los datos del la api para cada pagina en el momento en el cual la misma lo necesite. 
            //Puede que paresca redundante pero evita problemas de ejecucion. Con mas experiancia se optimiza
            var typesModelResponse = await DataService.GetInstance().GetParkingTypesModel();

            if (!typesModelResponse.IsSuccess)
            {
                DialogService.GetInstance().ShowInfoAlertOnMaster("Error message", typesModelResponse.Message);
                await NavigationService.GetInstance().BackOnMaster();
                return;
            }

            ParkingTypes = new ObservableCollection<ParkingTypeModel>(typesModelResponse.Result as List<ParkingTypeModel>); //TODO: Create generic Model Response
        }


        private async void GetParkingCategoriesAsync()
        {
            //APIService se puede utilizar directamente si del backend llega ya el Model y no la entidad.
            //Cargar los datos del la api para cada pagina en el momento en el cual la misma lo necesite. 
            //Puede que paresca redundante pero evita problemas de ejecucion. Con mas experiancia se optimiza
            var categoriesModelResponse = await DataService.GetInstance().GetParkingCategories();

            if (!categoriesModelResponse.IsSuccess) {
                DialogService.GetInstance().ShowInfoAlertOnMaster("Error message", categoriesModelResponse.Message);
                await NavigationService.GetInstance().BackOnMaster();
                return;
            }

            Categories = new ObservableCollection<ParkingCategoryModel>(categoriesModelResponse.Result as List<ParkingCategoryModel>); //TODO: Create generic Model Response
        }

        private void GetColorParkingIcon()
        {
            if (SelectedCategory.Category == "Business")
                IconNameBasedOnCategory = "ic_location_green";
            else
                IconNameBasedOnCategory = "ic_location_black";
        }
        private void GetParkingPrice()
        {
            if (SelectedCategory!=null && SelectedParkingType!=null)
            {
                //Revisar los tipos de parqueos en la BD.
                ParkingPrice = SelectedParkingType.Type == "For Month" ? SelectedCategory.MonthPrice : SelectedCategory.HourPrice;
            }
        }
        private async void CreateParkingEvent()
        {
            bool parkingIsByHours= false;
            if (SelectedParkingType == null)
            {
                await DialogService.GetInstance().ShowDialogAlertOnMaster(
                    "Alert",
                    "You must set the parking type before te aviability.");
                return;
            }
            //Revisar los tipos de parqueos en la BD.
            if (SelectedParkingType.Type == "By Hours")
            {
                parkingIsByHours = true;
            }
            MainViewModel.GetInstance().Event = new EventViewModel(parkingIsByHours);
            await _navigationService.NavigateOnMaster("EventView");
        }
    }
}