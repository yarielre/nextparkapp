using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using NextPark.Enums.Enums;
using NextPark.Mobile.Core.Settings;
using NextPark.Mobile.Services;
using NextPark.Mobile.Services.Data;
using NextPark.Models;
using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
    public class AddEventViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header username
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public DateTime StartDate { get; set; }     // Event start date
        public DateTime EndDate { get; set; }       // Event end date
        public TimeSpan StartTime { get; set; }     // Event timeslot start
        public TimeSpan EndTime { get; set; }       // Event timeslot end
        public bool TimeSlotVisible { get; set; }   // Timeslot visible
        public bool DeleteButtonVisible { get; set; }   // Delete button visible

        public bool IsRunning { get; set; }         // Activity spinner

        public Color AllDayTextColor { get; set; }  // All day textcolor
        public string AddButtonText { get; set; }   // Add button text
        public ICommand OnAddClick { get; set; }    // Add event action
        public ICommand OnDeleteClick { get; set; } // Delete event action

        private bool _allDaySwitchToggled { get; set; }
        public bool AllDaySwitchToggled                 // Parking active status value
        {
            get { return _allDaySwitchToggled; }
            set { _allDaySwitchToggled = value; AllDaySwitchToggledMethod(value); }
        }

        private EventModel _event { get; set; }
        private bool _modifying { get; set; }


        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly EventDataService _eventDataService;
        private ObservableCollection<string> Days;
        private ObservableCollection<string> Hours;
        private ObservableCollection<string> Minutes;

        // METHODS
        public AddEventViewModel(IDialogService dialogService,
                                 IApiService apiService,
                                 IAuthService authService,
                                 INavigationService navService,
                                 EventDataService eventDataService)
                                 : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _eventDataService = eventDataService;

            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnAddClick = new Command<object>(OnAddClickMethod);
            OnDeleteClick = new Command<object>(OnDeleteClickMethod);
            TimeSlotVisible = false;

            // Custom DateTimePicker
            Minutes = new ObservableCollection<string> {
                {"0" },{ "15"},{ "30"},{"45"}
            };
            Hours = new ObservableCollection<string>();
            for (int i = 0; i < 24; i++) {
                Hours.Add(i.ToString());
            }
            Days = new ObservableCollection<string>();
            DateTime dt = DateTime.Now;
            for (int i = 0; i < 365; i++)
            {
                Days.Add(dt.ToShortDateString());
                dt.AddDays(1.0);
            }
            base.OnPropertyChanged("Minutes");
        }

        public override Task InitializeAsync(object data = null)
        {
            if (data != null) {
                if (data is EventModel) {
                    _event = (EventModel)data;

                    // Header
                    BackText = "Indietro";
                    UserName = AuthSettings.User.Name;
                    UserMoney = AuthSettings.UserCoin.ToString("N0");
                    base.OnPropertyChanged("BackText");
                    base.OnPropertyChanged("UserName");
                    base.OnPropertyChanged("UserMoney");

                    if (_event.Id == 0) {
                        // new event
                        AllDaySwitchToggled = true;
                        AllDayTextColor = Color.Black;

                        StartDate = DateTime.Now.Date;
                        StartTime = DateTime.Now.TimeOfDay;
                        EndDate = DateTime.Now.Date;
                        EndTime = DateTime.Now.TimeOfDay;

                        DeleteButtonVisible = false;
                        AddButtonText = "Aggiungi";
                        _modifying = false;

                    } else {
                        // modify an event
                        StartDate = _event.StartDate.Date;
                        StartTime = _event.StartDate.TimeOfDay;
                        EndDate = _event.EndDate.Date;
                        EndTime = _event.EndDate.TimeOfDay;

                        if (_event.RepetitionType == RepetitionType.Dayly)
                        {
                            AllDaySwitchToggled = true;
                            AllDayTextColor = Color.Black;
                        }
                        DeleteButtonVisible = true;
                        AddButtonText = "Salva";
                        _modifying = true;
                    }

                    base.OnPropertyChanged("AllDaySwitchToggled");
                    base.OnPropertyChanged("AllDayTextColor");
                    base.OnPropertyChanged("StartDate");
                    base.OnPropertyChanged("StartTime");
                    base.OnPropertyChanged("EndDate");
                    base.OnPropertyChanged("EndTime");
                    base.OnPropertyChanged("DeleteButtonVisible");
                    base.OnPropertyChanged("AddButtonText");

                }
            }
            return Task.FromResult(false);
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

        // Add event button action
        public void OnAddClickMethod(object sender)
        {
            // TODO: fill data according to backend login method
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");
            // TODO: call login method
            AddEvent();
        }

        // Delete event button action
        public void OnDeleteClickMethod(object sender)
        {
            // TODO: fill data according to backend login method
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");
            // TODO: call login method
            DeleteEvent();
        }

        public async void AddEvent()
        {
            if (AllDaySwitchToggled) {
                StartTime = TimeSpan.FromHours(0.0);
                EndTime = TimeSpan.FromHours(23.0);
            }

            EventModel result;

            if (_modifying) {
                _event.StartDate = StartDate + StartTime;
                _event.EndDate = EndDate + EndTime;
                _event.RepetitionEndDate = EndDate;
                result = await _eventDataService.Put(_event, _event.Id);
            } else {
                _event.StartDate = StartDate + StartTime;
                _event.EndDate = EndDate + EndTime;
                _event.RepetitionEndDate = EndDate;
                result = await _eventDataService.Post(_event);
            }

            if (result != null) {
                //NavigationService.NavigateToAsync<ParkingDataViewModel>();
            }
        }

        public async void DeleteEvent()
        {
            // await oderRemove
            var result = await _eventDataService.Delete(_event.Id);
        }

        // Activate/Deactivate Parking toggle switch action
        public void AllDaySwitchToggledMethod(bool value)
        {
            if (value)
            {
                TimeSlotVisible = false;
            }
            else
            {
                TimeSlotVisible = true;
                AllDayTextColor = Color.Gray;
                base.OnPropertyChanged("AllDayTextColor");
            }
            base.OnPropertyChanged("TimeSlotVisible");
        }
    }
}
