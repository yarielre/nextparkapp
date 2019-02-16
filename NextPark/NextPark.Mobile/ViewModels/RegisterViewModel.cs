using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using NextPark.Models;

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

        public string Email { get; set; }           // User e-mail
        public string Password { get; set; }        // Password text
        public string PasswordConfirm { get; set; } // Password confirm text
        public string Name { get; set; }            // Name
        public string Lastname { get; set; }        // Lastname
        public string Phone { get; set; }           // Phone
        public string Address { get; set; }         // Address
        public string NPA { get; set; }             // NPA
        public string City { get; set; }            // City/Country
        public string CarPlate { get; set; }        // Car Plate

        /* Future implementation
        private ImageSource _userImage;             // user image
        public ImageSource UserImage
        {
            get => _userImage;
            set => SetValue(ref _userImage, value);
        }
        public ICommand OnUserImageTap { get; set; }    // User image action
        */

        public bool IsRunning { get; set; }             // Activity spinner
        public ICommand OnRegisterClick { get; set; }   // Register button action

        // SERVICES
        private readonly IDialogService _dialogService;

        // PRIVATE VARIBLES
        private bool dataCheckError;

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

            // Future implementation
            //OnUserImageTap = new Command<object>(OnUserImageTapMethod);
            //UserImage = "icon_add_photo_256.png";
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            // Header
            BackText = "Indietro";
            UserName = "Accedi";
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
            NavigationService.NavigateToAsync<LoginViewModel>();
        }

        // User Click action
        public void OnUserClickMethod(object sender)
        {
            // TODO: evaluate action (try to go to profile, do nothing?)
            NavigationService.NavigateToAsync<LoginViewModel>();
        }

        // Money Click action
        public void OnMoneyClickMethod(object sender)
        {
            // TODO: evaluate action (try to go to money page, do nothing?)
            NavigationService.NavigateToAsync<LoginViewModel>();
        }

        // Register button action
        public void OnRegisterClickMethod(object sender)
        {
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");
            RegisterMethod();
        }

        private async Task<bool> RegisterDataCheck()
        {
            bool error = false;

            // TODO: implemet all data check
            // E-mail
            if (!error && (string.IsNullOrEmpty(this.Email)|| !this.Email.Contains("@")))
            {
                await _dialogService.ShowAlert("Errore e-mail", "Inserire un indirizzo e-mail valido");
                error = true;
            }
            // Password
            if (!error && (string.IsNullOrEmpty(this.Password)))
            {
                // TODO: add other checks
                // No password inserted
                await _dialogService.ShowAlert("Errore Password", "La password deve essere lunga almeno n caratteri");
                error = true;
            }
            // Password confirm
            if (!error && ((this.PasswordConfirm == null) || (false == this.Password.Equals(this.PasswordConfirm))))
            {
                await _dialogService.ShowAlert("Le password sono differenti", "Il campo password e conferma password devono essere identici.");
                error = true;
            }
            // Name
            if (!error && (string.IsNullOrEmpty(this.Name)))
            {
                await _dialogService.ShowAlert("Errore Nome utente", "Il campo Nome è obbligatorio");
                error = true;
            }
            // Lastname
            if (!error && (string.IsNullOrEmpty(this.Lastname)))
            {
                await _dialogService.ShowAlert("Errore Nome utente", "Il campo Cognome è obbligatorio");
                error = true;
            }
            // Phone
            if (!error && (string.IsNullOrEmpty(this.Phone)))
            {
                await _dialogService.ShowAlert("Errore Targa", "Inserire una targa valida");
                error = true;
            }
            // Address
            if (!error && (string.IsNullOrEmpty(this.Address) || string.IsNullOrEmpty(this.NPA) || string.IsNullOrEmpty(this.City)))
            {
                await _dialogService.ShowAlert("Errore Targa", "Inserire un'indirizzo valido");
                error = true;
            }
            // Plate
            if (!error && ((this.CarPlate == null) || (this.CarPlate.Length == 0)))
            {
                await _dialogService.ShowAlert("Errore Targa", "Inserire una targa valida");
                error = true;
            }

            return error;
        }

        public async void RegisterMethod()
        {
            //Demo Login OK
            //var loginResponse = await AuthService.Login("demo@nextpark.ch", "Wisegar.1");


            // TODO: define user data

            // Register data check
            bool error = await RegisterDataCheck();
            if (error) 
            {
                // Stop activity spinner
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
                return;
            }

            try
            { 
                var demoUser = new RegisterModel
                {
                    Address = Address,
                    Cap = int.Parse(NPA),
                    City = City,
                    CarPlate = CarPlate,
                    Email = Email,
                    Lastname = Lastname,
                    Name = Name,
                    Phone = Phone,
                    Password = Password,
                    State = "CH",
                    UserName = Email
                };

                var registerResponse = await AuthService.Register(demoUser);

                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
                if ((registerResponse != null) && (registerResponse.IsSuccess)) {



                    await NavigationService.NavigateToAsync<HomeViewModel>();
                } else {
                    await _dialogService.ShowAlert("Alert", "Registrazione non riuscita");
                }
            } catch(Exception ex) {}
        }

        /* Future implementation
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
        */
    }
}
