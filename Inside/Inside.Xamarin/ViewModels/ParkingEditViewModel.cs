using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Inside.Domain.Models;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Models;
using Inside.Xamarin.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Inside.Xamarin.ViewModels
{
    public class ParkingEditViewModel : BaseViewModel
    {
        #region Constructor

        public ParkingEditViewModel(ParkingModel parking)
        {
            try
            {
                _navigationService = NavigationService.GetInstance();
                _parkingEdited = parking;
                SetImageSource(parking.ImageUrl);
                GetParkingCategories();
                GetParkingTypes();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
               
           
            //    GetColorParkingIcon();
        }

        #endregion

        #region Attributes

        private readonly NavigationService _navigationService;
        private MediaFile _mediaFile;
        private ImageSource _parkingPhoto;
        private ParkingModel _parkingEdited;
        private ObservableCollection<ParkingCategoryModel> _categories;
        private ObservableCollection<ParkingTypeModel> _parkingTypes;
        private ParkingCategoryModel _selectedCategory;
        private ParkingTypeModel _selectedParkingType;
        private string _iconNameBasedOnCategory;
        private double _parkingPrice;
        private bool _isRunning;
        private bool _isEnabled;

        #endregion

        #region Properties

        public ParkingModel ParkingEdited
        {
            get => _parkingEdited;
            set => SetValue(ref _parkingEdited, value);
        }

        public ParkingCategoryModel SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetValue(ref _selectedCategory, value);
                ParkingEdited.ParkingCategory = value;
                ParkingEdited.ParkingCategoryId = value.Id;
                GetColorParkingIcon();
                GetParkingPrice();
            }
        }
        public bool IsRunning
        {
            get => _isRunning;
            set => SetValue(ref _isRunning, value);
        }

        public double ParkingPrice
        {
            get => _parkingPrice;
            set => SetValue(ref _parkingPrice, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }

        public string IconNameBasedOnCategory
        {
            get => _iconNameBasedOnCategory;
            set => SetValue(ref _iconNameBasedOnCategory, value);
        }

        public ObservableCollection<ParkingCategoryModel> Categories
        {
            get => _categories;
            set => SetValue(ref _categories, value);
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
                ParkingEdited.ParkingType = value;
                ParkingEdited.ParkingTypeId = value.Id;
                GetParkingPrice();
            }
        }

        public ImageSource ParkingPhoto
        {
            get => _parkingPhoto;
            set => SetValue(ref _parkingPhoto, value);
        }

        #endregion

        #region Commands

        public ICommand ParkingEditCommand => new RelayCommand(ParkingEdit);
        public ICommand ChangePhotoCommand => new RelayCommand(ChangePhoto);
        public ICommand EventCommand => new RelayCommand(EditParkingEvent);

        #endregion


        #region Methods

        private void SetImageSource(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                ParkingPhoto = "add_photo";
                return;
            }
            ParkingPhoto = new UriImageSource
            {
                Uri = new Uri(string.Format("{0}{1}", HostSetting.BaseUrl, imageUrl))
            };
        }

        private async void EditParkingEvent()
        {
            bool isParkingByHours = ParkingEdited.ParkingType.Type == "By Hours";
            MainViewModel.GetInstance().Event = new EventViewModel(ParkingEdited.ParkingEvent,isParkingByHours);
            await _navigationService.NavigateOnMaster("EventView");
        }

        private async void ChangePhoto()
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

        private async void ParkingEdit()
        {
            byte[] imageArray = null;
            if (_mediaFile != null)
            {
                imageArray = FilesHelper.ReadFully(_mediaFile.GetStream());
                _mediaFile.Dispose();

                ParkingEdited.ImageBinary = imageArray;
            }
            IsRunning = true;
            var parkingResponse = await DataService.GetInstance().EditParking(ParkingEdited);
            IsRunning = false;
            if (!parkingResponse.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.GeneralError,
                    Languages.ParkingEditAlert,
                    Languages.GeneralAccept);
                
                //TO-CHECK: En este punto ya deberia de ser app_photo.
                ParkingPhoto = "add_photo";

                return;
            }

            var parkingEdited = parkingResponse.Result as ParkingModel;

            MessagingCenter.Send(parkingEdited, Messages.ParkingEdited);

            await NavigationService.GetInstance().BackOnMaster();
        }

        private async void GetParkingTypes()
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

            ParkingTypes =
                new ObservableCollection<ParkingTypeModel>(
                    typesModelResponse.Result as List<ParkingTypeModel>); //TODO: Create generic Model Response
            SelectedParkingType = ParkingTypes.FirstOrDefault(type => type.Id == ParkingEdited.ParkingTypeId);
        }


        private async void GetParkingCategories()
        {
            //APIService se puede utilizar directamente si del backend llega ya el Model y no la entidad.
            //Cargar los datos del la api para cada pagina en el momento en el cual la misma lo necesite.
            //Puede que paresca redundante pero evita problemas de ejecucion. Con mas experiancia se optimiza
            var categoriesModelResponse = await DataService.GetInstance().GetParkingCategories();
            if (!categoriesModelResponse.IsSuccess)
            {
                DialogService.GetInstance()
                    .ShowInfoAlertOnMaster("Error message", categoriesModelResponse.Message);
                await NavigationService.GetInstance().BackOnMaster();
                return;
            }

            Categories =
                new ObservableCollection<ParkingCategoryModel>(
                    categoriesModelResponse.Result as List<ParkingCategoryModel>); //TODO: Create generic Model Response
            SelectedCategory = Categories.FirstOrDefault(cat => cat.Id == ParkingEdited.ParkingCategoryId);
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
            if (ParkingEdited.ParkingCategory!=null && ParkingEdited.ParkingType!=null)
            {
                //Revisar los tipos de parqueos en la BD.
                ParkingPrice = ParkingEdited.ParkingType.Type == "For Month" ? SelectedCategory.MonthPrice : SelectedCategory.HourPrice;
            }
           
        }

        #endregion
    }
}