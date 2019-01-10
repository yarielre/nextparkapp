using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace NextPark.Mobile.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header username
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string RegisterName { get; set; }    // Username text
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
        public ICommand OnRegisterClick { get; set; }   // Register button action

        // SERVICES
        private readonly IDialogService _dialogService;

        // METHODS
        public RegisterViewModel(IDialogService dialogService,
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
            OnRegisterClick = new Command<object>(OnRegisterClickMethod);
            OnUserImageTap = new Command<object>(OnUserImageTapMethod);

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
            BackText = "Indietro";
            UserName = "Login";
            UserMoney = "0";
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            return Task.FromResult(false);
        }

        // Back Click action
        public void OnBackClickMethod(object sender)
        {
            // TODO: go back to previus view
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            // TODO: evaluate action (try to go to profile, do nothing?)
            //NavigationService.NavigateToAsync<UserProfileViewModel>();
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            // TODO: evaluate action (try to go to money page, do nothing?)
            NavigationService.NavigateToAsync<MoneyViewModel>();
        }

        // Register button action
        public void OnRegisterClickMethod(object sender)
        {
            // TODO: fill user data according to register data model
            // TODO: send registration request to backend
            _dialogService.ShowAlert("Alert", "TODO: Register user");
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
            } catch (Exception ex) {
                //TODO: manage exception here...
            }
        }
    }
}
