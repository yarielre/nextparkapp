using System;
using System.Windows.Input;
using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace NextPark.Mobile.ViewModels
{
    public class UserDataViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        //public string UserName { get; set; }      // Already declared for header
        public string Password { get; set; }        // Password text
        public string PasswordConfirm { get; set; } // Password confirm text
        public string Name { get; set; }            // Name
        public string Surname { get; set; }         // Surname
        public string Address { get; set; }         // Address
        public string NPA { get; set; }             // NPA
        public string City { get; set; }            // City/Country
        public string Plate { get; set; }           // Plate
        private ImageSource _userImage;             // user image
        public ImageSource UserImage
        {
            get => _userImage;
            set => SetValue(ref _userImage, value);
        }

        public ICommand OnUserImageTap { get; set; }    // User image action

        public bool IsRunning { get; set; }         // Activity spinner
        public ICommand OnSaveClick { get; set; }   // Save button click action

        // SERVICES
        private readonly IDialogService _dialogService;

        // PRIVATE VARIABLES
        private static bool activity = false;

        // METHODS
        public UserDataViewModel(IDialogService dialogService,
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
            OnUserImageTap = new Command<object>(OnUserImageTapMethod);
            OnSaveClick = new Command<object>(OnSaveClickMethod);

            UserImage = "icon_add_photo_256.png";
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
            BackText = "Profilo";
            UserName = "Jonny";
            UserMoney = "8";
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // User Data
            // TODO: insert user data
            // Password
            // PasswordConfirm
            // Name
            // Surname
            // Address
            // NPA
            // City
            // Plate

            return Task.FromResult(false);
        }

        // Back Click Action
        public void OnBackClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Text and Click action
        public void OnMoneyClickMethod(object sender)
        {
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // User image tap action
        public void OnUserImageTapMethod(object args)
        {
            AddPhoto();
        }

        private async void AddPhoto()
        {
            await CrossMedia.Current.Initialize();
            var source = await Application.Current.MainPage.DisplayActionSheet(
                "Scegli la fonte per aggiungere la foto.",
                "Annulla",
                null,
                "Galleria",
                "Fotocamera");
            if (source == null)
            {
                return;
            }
            if (source == "Fotocamera")
                TakeUserPhoto();
            if (source == "Galleria")
                PickUserPhoto();
        }

        // Take User Image
        private async void TakeUserPhoto()
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
            UserImage = ImageSource.FromStream(() => { return mediaFile.GetStream(); });
        }

        // Pick User Image
        private async void PickUserPhoto()
        {
            MediaFile mediaFile;
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Errore Galleria",
                    "Nessuna foto disponibile",
                    "OK");
                return;
            }
            try
            {
                mediaFile = await CrossMedia.Current.PickPhotoAsync();
                if (mediaFile == null)
                    return;
                UserImage = ImageSource.FromStream(() => { return mediaFile.GetStream(); });
            }
            catch (Exception ex)
            {
                //TODO: manage exception here...
            }
        }

        // Save button click action
        public void OnSaveClickMethod(object sender)
        {
            _dialogService.ShowAlert("Alert", "TODO: Update User");
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");
        }
    }
}
