using GalaSoft.MvvmLight.Command;
using Inside.Domain.Entities;
using Inside.Domain.Models;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Models;
using Inside.Xamarin.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Inside.Xamarin.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        #region Attributes
        private UserModel _currentUser;
        private ParkingModel _parkingRentedByHours;
        private ParkingModel _parkingRentedForMonth;
        //This Password must be in app settings;
        private readonly string _cipherPassword = "lazy.netprogrammer";
        #endregion

        #region Properties
        public UserModel CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
                if (value == null) return;
                Menu.UserName = value.UserName;
                Menu.Coins = value.Coins.ToString();
            }
        }

        public ParkingModel ParkingRentedByHours
        {
            get => _parkingRentedByHours;
            set => SetValue(ref _parkingRentedByHours, value);
        }
        //public List<ParkingCategoryModel> Categories { get; set; }
        //public List<ParkingTypeModel> ParkingTypes { get; set; }
        public ObservableCollection<MenuModel> Menus { get; set; }
        public CoinPageViewModel Coins { get; set; }
        public LoginViewModel Login { get; set; }
        public RegisterUserViewModel RegisterUser { get; set; }
        public ParkingCreateViewModel ParkingCreate { get; set; }
        public ParkingEditViewModel ParkingEdit { get; set; }
        public ParkingRentViewModel ParkingRent { get; set; }
        public MenuViewModel Menu { get; set; }
        public TabsPageViewModel Tabs { get; set; }
        public EventViewModel Event { get; set; }
        public EditProfileViewModel EditProfile { get; set; }
        public ChangePasswordViewModel ChangePassword { get; set; }
        public RentsByHoursViewModel RentsByHours { get; set; }
        public RentsForMonthViewModel RentsForMonth { get; set; }
        #endregion

        public MainViewModel()
        {
            _instance = this;
            LoadMenu();
            Login = new LoginViewModel();
            Coins = new CoinPageViewModel();
            RegisterUser = new RegisterUserViewModel();
            Menu = new MenuViewModel();
            Tabs = new TabsPageViewModel();
            //EditProfile = new EditProfileViewModel();
            ChangePassword = new ChangePasswordViewModel();

            MessagingCenter.Subscribe<object, string>(this, Messages.PushNotificationReceived, (sender, arg) => {
                //Services.DialogService.GetInstance().ShowInfoAlertOutMaster("Push Message received!", arg);
           });

        }

        public void SetUpLoginData(UserModel user, string authToken) // TODO: Move to DataService!!
        {
            InsideApi.AuthToken = authToken;
            CurrentUser = user;

            var userNameCipherText = Encrypt.EncryptString(CurrentUser.UserName, _cipherPassword);
            var userIdCipherText = Encrypt.EncryptString(CurrentUser.Id.ToString(), _cipherPassword);

            Settings.UserId = userIdCipherText;
            Settings.UserName = userNameCipherText;
            Settings.Token = authToken;
        }


        public async Task RestoreLoginData() // TODO: Move to DataService!!
        {
            if (CurrentUser != null) return;

            if (string.IsNullOrEmpty(Settings.Token) || string.IsNullOrEmpty(Settings.UserName))
            {
                NavigationService.GetInstance().SetMainPage(Pages.LoginView);
                return;
            }

            try
            {
                InsideApi.AuthToken = Settings.Token;
                var userName = Encrypt.DecryptString(Settings.UserName, _cipherPassword);
                var loggerUserResponse = await InsideApi.GetUserByUserName(HostSetting.AuthEndPoint, userName); // TODO: Move to AuthService!!

                if (loggerUserResponse == null ||
                    !loggerUserResponse.IsSuccess ||
                    loggerUserResponse.Result == null)
                {

                    NavigationService.GetInstance().SetMainPage(Pages.LoginView);
                    return;
                }
                CurrentUser = loggerUserResponse.Result as UserModel;
            }
            catch
            {
                NavigationService.GetInstance().SetMainPage(Pages.LoginView);
                return;
            }
        }

        public void ResetLoginData() // TODO: Move to DataService!!
        {
            InsideApi.AuthToken = string.Empty;
            CurrentUser = null;
            Settings.UserId = string.Empty;
            Settings.UserName = string.Empty;
            Settings.Token = string.Empty;
        }

        private void LoadMenu()  // TODO: Move to MasterViewModel
        {
            Menus = new ObservableCollection<MenuModel>
            {
                new MenuModel
                {
                    Title = Languages.MasterItemEditProfile,
                    Icon = "ic_settings",
                    PageName = "EditProfilePage",
                    OnTabActionCommand = new RelayCommand(this.EditProfileNavigate)
                },
                new MenuModel
                {
                    Title = Languages.MasterItemChangePassword,
                    Icon = "ic_settings",
                    PageName = "ChangePasswordPage",
                    OnTabActionCommand = new RelayCommand(this.ChangePasswordNavigate)
                },
                new MenuModel
                {
                    Title = Languages.MasterItemLogout,
                    Icon = "ic_exit_to_app",
                    PageName = "LoginPage",
                    OnTabActionCommand = new RelayCommand(this.Logout)
                }
            };
        }
        private async void Logout()
        {
            await Application.Current.MainPage.DisplayAlert(
                    Languages.MasterItemLogoutHeaderAlert,
                    Languages.MasterItemLogoutAlert,
                    Languages.GeneralAccept);

            //TODO: Implement backend logout call

            ResetLoginData();
            NavigationService.GetInstance().SetMainPage(Pages.LoginView);
        }

        private async void ChangePasswordNavigate()
        {
            //await Application.Current.MainPage.DisplayAlert(
            //        Languages.MasterItemChangePasswordHeaderAlert,
            //        Languages.MasterItemChangePasswordAlert,
            //        Languages.GeneralAccept);
            await NavigationService.GetInstance().NavigateOnMaster("ChangePassword");
        }
        private async void EditProfileNavigate()
        {
            //await Application.Current.MainPage.DisplayAlert(
            //       Languages.MasterItemEditProfileHeaderAlert,
            //     Languages.MasterItemEditProfileAlert,
            //       Languages.GeneralAccept, Languages.GeneralCancel);
            EditProfile = new EditProfileViewModel();
            await NavigationService.GetInstance().NavigateOnMaster("EditProfile");
        }

        public void PushMessageRecived(object message) {
            //TODO: Implement action on push notification recived!
        }
        
        private static MainViewModel _instance;
        public static MainViewModel GetInstance()
        {
            return _instance ?? new MainViewModel();
        }
    }
}