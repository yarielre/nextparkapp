using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using NextPark.Mobile.Extensions;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using NextPark.Mobile.Core.Settings;
using NextPark.Mobile.Services.Data;
using NextPark.Models;
using NextPark.Enums.Enums;

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

        private int index;
        public int Index
        {
            get { return index; }
            set { index = value; }
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

    public class UserParkingViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header user text
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public bool NoElementFound { get; set; }
        public double ParkingListHeight { get; set; }   // Parking List View Height Request

        public ICommand OnAddParking { get; set; }      // Add parking button action
        public ICommand OnParkingTapped { get; set; }   // Parking list tap action

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly ParkingDataService _parkingDataService;

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
                                    INavigationService navService,
                                    ParkingDataService parkingDataService)
                                    : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _parkingDataService = parkingDataService;

            // Header
            UserName = AuthSettings.UserName;
            UserMoney = AuthSettings.UserCoin.ToString("N0");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            OnAddParking = new Command<object>(OnAddParkingMethod);
            OnParkingTapped = new Command<object>(OnParkingTappedMethod);

            ParkingList = new ObservableCollection<ParkingItem>();
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

            // TODO: add parking list
            UpdateUserParkingData();

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

        // Updated UserParkings
        public async void UpdateUserParkingData()
        {
            await GetUserParkings();
        }

        private async Task GetUserParkings()
        {
            var parkingList = await _parkingDataService.Get();

            ParkingList.Clear();

            // Search user parkings 
            // TODO: use a filter on Parkings
            if (parkingList != null)
            {
                int count = 0;
                foreach (ParkingModel parking in parkingList)
                {
                    if (parking.UserId == int.Parse(AuthSettings.UserId))
                    {
                        bool free = false;
                        // TODO: check parking orders
                        if (parking.Status == ParkingStatus.Enabled)
                        {
                            free = true;
                        }
                        ParkingList.Add(new ParkingItem
                        {
                            UID = parking.Id,
                            Index = count++,
                            Address = parking.Address,
                            City = parking.Cap.ToString() + " " + parking.City,
                            Status = (free) ? "libero" : "occupato",
                            StatusColor = (free) ? Color.Green : Color.Red,
                            OnParkingTap = OnParkingTapped
                        });
                    }
                }
                base.OnPropertyChanged("ParkingList");
                if (ParkingList.Count == 0)
                {
                    NoElementFound = true;

                }
                else
                {
                    NoElementFound = false;
                }
                base.OnPropertyChanged("NoElementFound");

                ParkingListHeight = ParkingList.Count * 70.0;
                base.OnPropertyChanged("ParkingListHeight");
            }
        }
    }
}
