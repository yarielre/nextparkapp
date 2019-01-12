using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using NextPark.Mobile.Core.Settings;

namespace NextPark.Mobile.ViewModels
{
    public class ParkingItem
    {
        private int uid;
        public int UID
        {
            get { return uid; }
            set { uid = value; }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private string city;
        public string City
        {
            get { return city; }
            set { city = value; }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        private Color statusColor;
        public Color StatusColor
        {
            get { return statusColor; }
            set { statusColor = value; } 
        }

        private ICommand onParkingTap;
        public ICommand OnParkingTap
        {
            get { return onParkingTap; }
            set { onParkingTap = value; }
        }
    }

    public class UserParkingViewModel: BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public double ParkingListHeight { get; set; }   // Parking List View Height Request

        public ICommand OnAddParking { get; set; }      // Add parking button action
        public ICommand OnParkingTapped { get; set; }   // Parking list tap action

        // SERVICES
        private readonly IDialogService _dialogService;

        // PRIVATE VARIABLES
        private ObservableCollection<ParkingItem> _parkingList;

        public ObservableCollection<ParkingItem> ParkingList
        {
            get { return _parkingList; }
            set { _parkingList = value; base.OnPropertyChanged("ParkingList"); }
        }

        // METHODS
        public UserParkingViewModel(IDialogService dialogService,
                                    IApiService apiService,
                                    IAuthService authService,
                                    INavigationService navService)
                                    : base(apiService, authService, navService)
        {
            _dialogService = dialogService;

            // Header
            UserName = AuthSettings.UserName;
            UserMoney = AuthSettings.UserCoin.ToString("N0");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            OnAddParking = new Command<object>(OnAddParkingMethod);
            OnParkingTapped = new Command<object>(OnParkingTappedMethod);

            ParkingList = new ObservableCollection<ParkingItem>
            {
                new ParkingItem { UID = 0, Address = "Via Strada 1", City = "Lugano, Ticino", Status = "libero", StatusColor = Color.Green, OnParkingTap = OnParkingTapped},
                new ParkingItem { UID = 1, Address = "Via Strada 1.5", City = "Lugano, Ticino", Status = "libero", StatusColor = Color.Green, OnParkingTap = OnParkingTapped},
                new ParkingItem { UID = 2, Address = "Via Strada 2", City = "Lugano, Ticino", Status = "libero", StatusColor = Color.Green, OnParkingTap = OnParkingTapped}
            };
            base.OnPropertyChanged("ParkingList");

            ParkingListHeight = ParkingList.Count * 70.0;
            base.OnPropertyChanged("ParkingListHeight");
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
            UserName = AuthSettings.UserName;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("BackText");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // TODO: add parking list


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

        // Add Parking Click Action
        public void OnAddParkingMethod(object sender)
        {
            NavigationService.NavigateToAsync<AddParkingViewModel>();
            //_dialogService.ShowAlert("Alert", "Add Parking");
        }

        // Parking list item click Action
        public void OnParkingTappedMethod(object sender)
        {
            // TODO: Evaluate using UID or index of parking list (currently using UID as index)
            if (sender is int)
            {
                ParkingItem item = ParkingList[(int)sender];
                // TODO: pass parking item to parking data page
                NavigationService.NavigateToAsync<ParkingDataViewModel>(item);
                //_dialogService.ShowAlert("Alert", "Parking data: " + item.Address);
            }
        }
    }
}
