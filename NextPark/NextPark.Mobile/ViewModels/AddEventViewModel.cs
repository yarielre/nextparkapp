using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using NextPark.Enums;
using NextPark.Enums.Enums;
using NextPark.Mobile.Services;
using NextPark.Mobile.Services.Data;
using NextPark.Mobile.UIModels;
using NextPark.Models;
using NextPark.Mobile.Settings;
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

        private DateTime _startDate { get; set; }
        public DateTime StartDate                   // Event start date
        {
            get { return _startDate; }
            set 
            {   _startDate = value;
                if (_startDate > RepetitionEndDate) { 
                    RepetitionEndDate = StartDate;
                    base.OnPropertyChanged("RepetitionEndDate");
                }
                RepetitionMinEndDate = StartDate;
                base.OnPropertyChanged("RepetitionMinEndDate");
            } 
        }
        public DateTime MinStartDate { get; set; }  // Minimum event start date
        public DateTime EndDate { get; set; }       // Event end date
        public DateTime RepetitionEndDate { get; set; }
        public DateTime RepetitionMinEndDate { get; set; }
        public TimeSpan StartTime { get; set; }     // Event timeslot start
        public TimeSpan EndTime { get; set; }       // Event timeslot end
        public int RepetitionIndex                  // Repetition Selected Index
        {
            get { return _repetitionIndex; }
            set { _repetitionIndex = value; OnRepetitionChangedMethod(_repetitionIndex); } 
        }
        public bool RepetitionEndVisible { get; set; }  // Repetition end visibility

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

        private ObservableCollection<UISelectionItem> _weekDays;
        public ObservableCollection<UISelectionItem> WeekDays {
            get { return _weekDays; }
            set { _weekDays = value; base.OnPropertyChanged("WeekDays"); }
        }
        public bool WeekDayVisible { get; set; }

        // PRIVATE VARIABLES
        private EventModel _event { get; set; }
        private bool _modifying { get; set; }
        private int _repetitionIndex { get; set; }

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly EventDataService _eventDataService;

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

            RepetitionIndex = 0;
            RepetitionEndVisible = false;
            WeekDayVisible = false;

            WeekDays = new ObservableCollection<UISelectionItem>();
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
                        RepetitionEndDate = StartDate;
                        _event.RepetitionType = RepetitionType.Dayly;
                        /*
                        _event.WeekRepeat = new List<MyDayOfWeek>
                        {
                            (MyDayOfWeek)(DateTime.Now.DayOfWeek)
                        };
                        */
                        RepetitionIndex = 0;
                        DeleteButtonVisible = false;
                        AddButtonText = "Aggiungi";
                        _modifying = false;

                    } else {
                        // modify an event
                        StartDate = _event.StartDate.Date;
                        StartTime = _event.StartDate.TimeOfDay;
                        EndDate = _event.EndDate.Date;
                        EndTime = _event.EndDate.TimeOfDay;
                        RepetitionEndDate = _event.RepetitionEndDate;
                        /*
                        if (_event.WeekRepeat == null) {
                            _event.WeekRepeat = new List<MyDayOfWeek>
                            {
                                (MyDayOfWeek)(DateTime.Now.DayOfWeek)
                            };
                        }
                        */
                        DeleteButtonVisible = true;
                        AddButtonText = "Salva";
                        _modifying = true;
                    }
                    MinStartDate = DateTime.Now;
                    RepetitionMinEndDate = StartDate;

                    if ((_event.RepetitionType == RepetitionType.Dayly) && (RepetitionEndDate != StartDate))
                    {
                        RepetitionIndex = 1;
                        RepetitionEndVisible = true;
                    }
                    else if (_event.RepetitionType == RepetitionType.Weekly)
                    {
                        WeekDayVisible = true;
                        RepetitionIndex = 2;
                    }

                    base.OnPropertyChanged("AllDaySwitchToggled");
                    base.OnPropertyChanged("AllDayTextColor");
                    base.OnPropertyChanged("StartDate");
                    base.OnPropertyChanged("MinStartDate");
                    base.OnPropertyChanged("StartTime");
                    base.OnPropertyChanged("EndDate");
                    base.OnPropertyChanged("EndTime");
                    base.OnPropertyChanged("RepetitionEndVisible");
                    base.OnPropertyChanged("RepetitionIndex");
                    base.OnPropertyChanged("RepetitionEndDate");
                    base.OnPropertyChanged("RepetitionMinEndDate");
                    base.OnPropertyChanged("WeekDaysVisible");
                    base.OnPropertyChanged("DeleteButtonVisible");
                    base.OnPropertyChanged("AddButtonText");

                    WeekDays.Add(new UISelectionItem { Text = "Lunedì", Selected = false, Id = 1});
                    WeekDays.Add(new UISelectionItem { Text = "Martedì", Selected = false, Id = 2});
                    WeekDays.Add(new UISelectionItem { Text = "Mercoledì", Selected = false, Id = 3 });
                    WeekDays.Add(new UISelectionItem { Text = "Giovedì", Selected = false, Id = 4 });
                    WeekDays.Add(new UISelectionItem { Text = "Venerdì", Selected = false, Id = 5 });
                    WeekDays.Add(new UISelectionItem { Text = "Sabato", Selected = false, Id = 6 });
                    WeekDays.Add(new UISelectionItem { Text = "Domenica", Selected = false, Id = 0 });
                    /*
                    foreach (MyDayOfWeek dayOfWeek in _event.WeekRepeat) {
                        int index = ((int)dayOfWeek + 6) % 7;
                        WeekDays[index].Selected = true;
                    }
                    */

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

        // Add event button action
        public void OnRepetitionChangedMethod(int data)
        {
            int index = RepetitionIndex;
            switch (index) {
                default:
                case 0: // Never
                    RepetitionEndVisible = false;
                    WeekDayVisible = false;
                    break;
                case 1: // Daily
                    RepetitionEndVisible = true;
                    WeekDayVisible = false;
                    break;
                case 2: // Weekly
                    RepetitionEndVisible = true;
                    WeekDayVisible = true;
                    break;
            }
            base.OnPropertyChanged("RepetitionEndVisible");
            base.OnPropertyChanged("WeekDayVisible");
        }

        // Week Day Selected
        public void OnWeekDaySelected(object item)
        {
            UISelectionItem selItem = item as UISelectionItem;
            if (selItem.Selected == true) {
                selItem.Selected = false;
                MyDayOfWeek day = (MyDayOfWeek)selItem.Id;
                int index = (selItem.Id + 6) % 7;
                //_event.WeekRepeat.Remove((MyDayOfWeek)selItem.Id);
            } else {
                selItem.Selected = true;
                MyDayOfWeek day = (MyDayOfWeek)selItem.Id;

                //_event.WeekRepeat.Add((MyDayOfWeek)selItem.Id);
            }
            base.OnPropertyChanged("WeekDays");
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

            // Check Data
            if (StartTime > EndTime) {
                await _dialogService.ShowAlert("Errore", "L'orario di fine deve essere maggiore a quello di inizio");
                return;
            }

            switch (RepetitionIndex)
            {
                default:
                case 0: // Never
                    _event.RepetitionType = RepetitionType.Dayly;
                    RepetitionEndDate = StartDate;
                    break;
                case 1: // Daily
                    _event.RepetitionType = RepetitionType.Dayly;
                    break;
                case 2: // Weekly
                    _event.RepetitionType = RepetitionType.Weekly;
                    break;
            }

            if (_modifying) {
                _event.StartDate = StartDate + StartTime;
                _event.EndDate = StartDate + EndTime;
                _event.RepetitionEndDate = RepetitionEndDate;
                result = await _eventDataService.Put(_event, _event.Id);
            } else {
                _event.StartDate = StartDate + StartTime;
                _event.EndDate = StartDate + EndTime;
                _event.RepetitionEndDate = RepetitionEndDate;
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
