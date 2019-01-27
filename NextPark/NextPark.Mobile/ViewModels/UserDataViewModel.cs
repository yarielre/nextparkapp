using System;
using System.Windows.Input;
using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using NextPark.Mobile.Core.Settings;
using NextPark.Models;
using System.Collections.Generic;

namespace NextPark.Mobile.ViewModels
{
    public class UserDataViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        private ImageSource _userIcon;
        public ImageSource UserIcon
        {
            get => _userIcon;
            set => SetValue(ref _userIcon, value);
        }

        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string Email { get; set; }           // E-mail
        public string Password { get; set; }        // Password text
        public string PasswordConfirm { get; set; } // Password confirm text
        public string Name { get; set; }            // Name
        public string Lastname { get; set; }        // Lastname
        public string Phone { get; set; }           // Phone
        public string Address { get; set; }         // Address
        public string NPA { get; set; }             // NPA
        public string City { get; set; }            // City/Country
        public string CarPlate { get; set; }        // CarPlate

        /* Future implementation
        private ImageSource _userImage;             // user image
        public ImageSource UserImage
        {
            get => _userImage;
            set => SetValue(ref _userImage, value);
        }
        public ICommand OnUserImageTap { get; set; }    // User image action
        */

        public bool IsRunning { get; set; }         // Activity spinner
        public ICommand OnSaveClick { get; set; }   // Save button click action

        private EditProfileModel editModel { get; set; }

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IProfileService _profileService;

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
            _profileService = new ProfileService(apiService);

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            OnSaveClick = new Command<object>(OnSaveClickMethod);

            UserName = AuthSettings.UserName;
            UserMoney = AuthSettings.UserCoin.ToString("N0");

            /* Future implementation
            OnUserImageTap = new Command<object>(OnUserImageTapMethod);
            UserImage = "icon_add_photo_256.png";
            */
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            // Header
            BackText = "Profilo";
            UserName = AuthSettings.UserName;
            UserMoney = AuthSettings.UserCoin.ToString("N0");

            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            if (AuthService.IsUserAuthenticated())
                // User Data
                // TODO: insert user data
                // Password
                // PasswordConfirm
                // Name
                // Lastname
                // Phone
                // Address
                // NPA
                // City
                // Plate
                GetUserData();

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

        /* Future implementation
        // User image tap action
        public void OnUserImageTapMethod(object args)
        {
            AddPhoto();
        }
        */

        private async void GetUserData()
        {
            UserModel userData = AuthSettings.User;

            string[] addressElements = userData.Address.Split(',');
            if (addressElements.Length > 2) {
                Address = addressElements[0];
                NPA = addressElements[1];
                City = addressElements[2];
            }
            Email = userData.Email;
            Name = userData.Name;
            Lastname = userData.Lastname;
            CarPlate = userData.CarPlate;

            base.OnPropertyChanged("Email");
            base.OnPropertyChanged("Name");
            base.OnPropertyChanged("Lastname");
            base.OnPropertyChanged("Phone");
            base.OnPropertyChanged("Address");
            base.OnPropertyChanged("NPA");
            base.OnPropertyChanged("City");
            base.OnPropertyChanged("CarPlate");         
        }

        /*
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
            UserIcon = ImageSource.FromStream(() => { return mediaFile.GetStream(); });
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
        */

        // Save button click action
        public void OnSaveClickMethod(object sender)
        {
            // Prepare user model to update user data
            string userAddress = this.Address + "," + this.NPA + "," + this.City;
            this.editModel = new EditProfileModel
            {
                Username = this.UserName,
                Name = this.Name,
                Lastname = this.Lastname,
                Email = this.Email,
                Address = userAddress,
                State = "CH",
                CarPlate = this.CarPlate   
            };

            // Start Activity spinner
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");

            // Update user data
            UpdateUserData(editModel);

        }

        public async void UpdateUserData(EditProfileModel model)
        {
            if (model != null) {
                try {
                    var response = await _profileService.UpdateUserData(editModel);

                    // Check update result
                    if (response != null)
                    {
                        AuthSettings.User.CarPlate = response.CarPlate;
                        AuthSettings.User.Address = response.Address;
                        AuthSettings.User.Name = response.Name;
                        AuthSettings.User.Lastname = response.Lastname;

                        CarPlate = AuthSettings.User.CarPlate;
                        Name = AuthSettings.User.Name;
                        Lastname = AuthSettings.User.Lastname;

                        string[] addressElements = AuthSettings.User.Address.Split(',');
                        if (addressElements.Length > 2)
                        {
                            Address = addressElements[0];
                            NPA = addressElements[1];
                            City = addressElements[2];
                        }

                        base.OnPropertyChanged("Name");
                        base.OnPropertyChanged("Lastname");
                        //base.OnPropertyChanged("Phone");
                        base.OnPropertyChanged("Address");
                        base.OnPropertyChanged("NPA");
                        base.OnPropertyChanged("City");
                        base.OnPropertyChanged("CarPlate");
                    }
                    else
                    {
                        await _dialogService.ShowAlert("Attenzione", "Acquisto fallito");
                    }

                } catch (Exception ex) {

                } finally {
                    // Stop activity spinner
                    IsRunning = false;
                    base.OnPropertyChanged("IsRunning");
                }
            }
        }
    }
}
