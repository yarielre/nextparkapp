using System;
using System.Windows.Input;
using NextPark.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using NextPark.Mobile.Core.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NextPark.Models;

namespace NextPark.Mobile.ViewModels
{
    public class Event {
        public string Text { get; set; }
        public Color TextColor { get; set; }
        public int StartSeconds { get; set; }
        public int DurationSeconds { get; set; }
        public Color EventColor { get; set; }
        public Constraint yConstPosition { get; set; }
        public Constraint xConstPosition { get; set; }
    }

    public class Order
    {
        public string Text { get; set; }
        public int StartSeconds { get; set; }
        public int DurationSeconds { get; set; }
        public Constraint yConstPosition { get; set; }
    }

    public class ParkingDataViewModel : BaseViewModel
    {
        // PROPERTIES
        public string BackText { get; set; }        // Header back text
        public ICommand OnBackClick { get; set; }   // Header back action
        public string UserName { get; set; }        // Header username
        public ICommand OnUserClick { get; set; }   // Header user action
        public string UserMoney { get; set; }       // Header money value
        public ICommand OnMoneyClick { get; set; }  // Header money action

        public string Address { get; set; }             // Parking address text
        public string City { get; set; }                // Parking city text
        public string ActiveStatusText { get; set; }    // Parking active status text
        public bool ActiveSwitchToggled                 // Parking active status value
        {
            get { return this.activeSwitchToggled; }
            set { activeSwitchToggled = value; OnSwitchToggleMethod(value); }
        }
        public ICommand OnEditParking { get; set; }     // Edit parking button action
        public ICommand OnCalendar { get; set; }        // Calendar click action
        public ICommand OnAddAvailability { get; set; } // Add Availability click action

        public DateTime SelectedDay { get; set; }       // Currently selected day
        private DateTime _pickerDateTime;
        public DateTime DatePickerDate
        {
            get { return _pickerDateTime; }
            set { if (!value.Equals(SelectedDay)) ChangeSelectedDay(value); }
        }
        // DatePicker date
        public ICommand OnPreviousWeek { get; set; }    // Previous week click action
        public ICommand OnNextWeek { get; set; }        // Next week click action
        public ICommand OnDaySelected { get; set; }     // Day number selection

        public string Day1Text { get; set; }            // Monday day numer text
        public Color Day1TextColor { get; set; }        // Monday number text color
        public Color Day1BackgroundColor { get; set; }  // Monday number background color
        public DateTime Day1DateTime { get; set; }      // Monday date/time
        public string Day2Text { get; set; }            // Tuesday day number text
        public Color Day2TextColor { get; set; }        // Tuesday number text color
        public Color Day2BackgroundColor { get; set; }  // Tuesday number background color
        public DateTime Day2DateTime { get; set; }      // Tuesday date/time
        public string Day3Text { get; set; }            // Wednesday day number text 
        public Color Day3TextColor { get; set; }        // Wednesday number text color
        public Color Day3BackgroundColor { get; set; }  // Wednesday number background color
        public DateTime Day3DateTime { get; set; }      // Wednesday date/time
        public string Day4Text { get; set; }            // Thursday day numer text
        public Color Day4TextColor { get; set; }        // Thursday number text color
        public Color Day4BackgroundColor { get; set; }  // Thursday number background color
        public DateTime Day4DateTime { get; set; }      // Thursday date/time
        public string Day5Text { get; set; }            // Friday day numer text
        public Color Day5TextColor { get; set; }        // Friday number text color
        public Color Day5BackgroundColor { get; set; }  // Friday number background color
        public DateTime Day5DateTime { get; set; }      // Friday date/time
        public string Day6Text { get; set; }            // Saturday day numer text
        public Color Day6TextColor { get; set; }        // Saturday number text color
        public Color Day6BackgroundColor { get; set; }  // Saturday number background color
        public DateTime Day6DateTime { get; set; }      // Saturday date/time
        public string Day7Text { get; set; }            // Sunday day numer text
        public Color Day7TextColor { get; set; }        // Sunday number text color
        public Color Day7BackgroundColor { get; set; }  // Sunday number background color
        public DateTime Day7DateTime { get; set; }      // Sunday date/time

        // SERVICES
        private readonly IDialogService _dialogService;

        // PRIVATE VARIABLES
        private ParkingItem parking;
        private bool activeSwitchToggled;
        private ObservableCollection<Event> events;
        public ObservableCollection<Event> Events
        {
            get { return events; }
            set { events = value; base.OnPropertyChanged("Events"); }
        }
        private ObservableCollection<Order> orders;
        public ObservableCollection<Order> Orders
        {
            get { return orders; }
            set { orders = value; base.OnPropertyChanged("Orders"); }
        }


        // METHODS
        public ParkingDataViewModel(IDialogService dialogService,
                                    IApiService apiService,
                                    IAuthService authService,
                                    INavigationService navService)
                                    : base(apiService, authService, navService)
        {
            _dialogService = dialogService;

            // Header
            UserName = AuthSettings.UserName;
            UserMoney = AuthSettings.UserCoin.ToString("N0");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);

            // Buttons action
            OnEditParking = new Command<object>(OnEditParkingMethod);
            OnCalendar = new Command<object>(OnCalendarMethod);
            OnAddAvailability = new Command<object>(OnAddAvailabilityMethod);

            // Calendar actions
            OnPreviousWeek = new Command<object>(OnPreviousWeekMethod);
            OnNextWeek = new Command<object>(OnNextWeekMethod);
            OnDaySelected = new Command<object>(OnDaySelectedMethod);
            //OnDateSelected = new Command<object>(OnDateSelected);
        }

        // Initialization
        public override Task InitializeAsync(object data = null)
        {
            if (data == null)
            {
                return Task.FromResult(false);
            }

            if (data is ParkingItem)
            {
                parking = (ParkingItem)data;

                // Header
                BackText = "Parcheggi";
                UserName = AuthSettings.UserName;
                UserMoney = AuthSettings.UserCoin.ToString("N0");
                base.OnPropertyChanged("BackText");
                base.OnPropertyChanged("UserName");
                base.OnPropertyChanged("UserMoney");

                Address = parking.Address;
                base.OnPropertyChanged("Address");
                City = parking.City;
                base.OnPropertyChanged("City");

                // Default Value
                ActiveStatusText = "Attivo";
                ActiveSwitchToggled = true;
                base.OnPropertyChanged("ActiveStatusText");
                base.OnPropertyChanged("ActiveSwitchToggled");
            }

            ChangeSelectedDay(DateTime.Now);

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

        // Edit Parking Click action
        public void OnEditParkingMethod(object sender)
        {
            // TODO: Evaluate Add Parking page with element or EditParking Page to be added
            NavigationService.NavigateToAsync<AddParkingViewModel>(parking);
        }

        // Activate/Deactivate Parking toggle switch action
        public void OnSwitchToggleMethod(bool value)
        {
            if (value)
            {
                ActiveStatusText = "Attivo";
            }
            else
            {
                ActiveStatusText = "Non Attivo";
            }
            base.OnPropertyChanged("ActiveStatusText");
        }

        // Calendar Click action
        public void OnCalendarMethod(object sender)
        {
            // TODO: Open calendar and manage selected date
            _dialogService.ShowAlert("Alert", "TODO: open calendar");
        }

        // Add Availability Click action
        public void OnAddAvailabilityMethod(object sender)
        {
            // TODO: Add availability
            NavigationService.NavigateToAsync<AddEventViewModel>(new EventModel { ParkingId = parking.UID});
        }

        // Go to previous week
        public void OnPreviousWeekMethod(object data = null)
        {
            ChangeSelectedDay(SelectedDay.AddDays(-7.0));
        }

        // Go to next week
        public void OnNextWeekMethod(object data = null) 
        {
            ChangeSelectedDay(SelectedDay.AddDays(7.0));
        }

        // Day of the week selected
        public void OnDaySelectedMethod(object data)
        {
            if (data is DateTime) {
                ChangeSelectedDay((DateTime)data);
            }
        }

        void OnDateSelectedMethod(object sender, DateChangedEventArgs args)
        {
            ChangeSelectedDay(args.NewDate);
        }

        // Select day
        public void ChangeSelectedDay(DateTime dateTime)
        {
            SelectedDay = dateTime;
            _pickerDateTime = dateTime;
            base.OnPropertyChanged("DatePickerDate");

            // Deselect all days
            Day1TextColor = Color.Gray;
            Day1BackgroundColor = Color.Transparent;
            Day2TextColor = Color.Gray;
            Day2BackgroundColor = Color.Transparent;
            Day3TextColor = Color.Gray;
            Day3BackgroundColor = Color.Transparent;
            Day4TextColor = Color.Gray;
            Day4BackgroundColor = Color.Transparent;
            Day5TextColor = Color.Gray;
            Day5BackgroundColor = Color.Transparent;
            Day6TextColor = Color.Gray;
            Day6BackgroundColor = Color.Transparent;
            Day7TextColor = Color.Gray;
            Day7BackgroundColor = Color.Transparent;

            int weekDay = (int)dateTime.DayOfWeek;

            switch(weekDay) {
                default:
                case 1: Day1TextColor = Color.White; Day1BackgroundColor = Color.FromHex("#8CC63F"); break;
                case 2: Day2TextColor = Color.White; Day2BackgroundColor = Color.FromHex("#8CC63F"); break;
                case 3: Day3TextColor = Color.White; Day3BackgroundColor = Color.FromHex("#8CC63F"); break;
                case 4: Day4TextColor = Color.White; Day4BackgroundColor = Color.FromHex("#8CC63F"); break;
                case 5: Day5TextColor = Color.White; Day5BackgroundColor = Color.FromHex("#8CC63F"); break;
                case 6: Day6TextColor = Color.White; Day6BackgroundColor = Color.FromHex("#8CC63F"); break;
                case 0: Day7TextColor = Color.White; Day7BackgroundColor = Color.FromHex("#8CC63F"); break;
            }

            if (weekDay == 0) weekDay = 7;
            weekDay--;


            int[] dayText = new int[7];
            DateTime[] dayDateTime = new DateTime[7];

            DateTime setDateTime = dateTime;

            for (int i = 0; i < 7; i++) {
                int diff = i - weekDay;
                TimeSpan time = TimeSpan.FromDays(diff);
                setDateTime = dateTime.AddDays((double)diff);
                dayText[i] = setDateTime.Day;
                dayDateTime[i] = setDateTime;
            }

            Day1Text = dayText[0].ToString();
            Day1DateTime = dayDateTime[0];
            Day2Text = dayText[1].ToString();
            Day2DateTime = dayDateTime[1];
            Day3Text = dayText[2].ToString();
            Day3DateTime = dayDateTime[2];
            Day4Text = dayText[3].ToString();
            Day4DateTime = dayDateTime[3];
            Day5Text = dayText[4].ToString();
            Day5DateTime = dayDateTime[4];
            Day6Text = dayText[5].ToString();
            Day6DateTime = dayDateTime[5];
            Day7Text = dayText[6].ToString();
            Day7DateTime = dayDateTime[6];

            // Update Interface
            base.OnPropertyChanged("Day1Text");
            base.OnPropertyChanged("Day1TextColor");
            base.OnPropertyChanged("Day1BackgroundColor");
            base.OnPropertyChanged("Day1DateTime");
            base.OnPropertyChanged("Day2Text");
            base.OnPropertyChanged("Day2TextColor");
            base.OnPropertyChanged("Day2BackgroundColor");
            base.OnPropertyChanged("Day2DateTime");
            base.OnPropertyChanged("Day3Text");
            base.OnPropertyChanged("Day3TextColor");
            base.OnPropertyChanged("Day3BackgroundColor");
            base.OnPropertyChanged("Day3DateTime");
            base.OnPropertyChanged("Day4Text");
            base.OnPropertyChanged("Day4TextColor");
            base.OnPropertyChanged("Day4BackgroundColor");
            base.OnPropertyChanged("Day4DateTime");
            base.OnPropertyChanged("Day5Text");
            base.OnPropertyChanged("Day5TextColor");
            base.OnPropertyChanged("Day5BackgroundColor");
            base.OnPropertyChanged("Day5DateTime");
            base.OnPropertyChanged("Day6Text");
            base.OnPropertyChanged("Day6TextColor");
            base.OnPropertyChanged("Day6BackgroundColor");
            base.OnPropertyChanged("Day6DateTime");
            base.OnPropertyChanged("Day7Text");
            base.OnPropertyChanged("Day7TextColor");
            base.OnPropertyChanged("Day7BackgroundColor");
            base.OnPropertyChanged("Day7DateTime");

            RefreshEvents(dateTime);
        }

        public void RefreshEvents(DateTime dateTime) 
        {
            // Clear all previous events
            if (Events != null) Events.Clear();

            Events = new ObservableCollection<Event>
            {
                new Event { Text = "Disponibile", StartSeconds=7, DurationSeconds=60, EventColor=Color.FromHex("#8CC63F"), TextColor=Color.DarkGreen, yConstPosition=Constraint.RelativeToParent((parent) => {return parent.Y + 7 + 0;}), xConstPosition=Constraint.RelativeToParent((parent) => {return parent.Width*0.025;})},
                new Event { Text = "Disponibile", StartSeconds=187, DurationSeconds=30, EventColor=Color.FromHex("#8CC63F"), TextColor=Color.DarkGreen, yConstPosition=Constraint.RelativeToParent((parent) => {return parent.Y + 7 + 180;}), xConstPosition=Constraint.RelativeToParent((parent) => {return parent.Width*0.025;})},
                new Event { Text = "TI 12345", StartSeconds=37, DurationSeconds=30, EventColor=Color.LightSteelBlue, TextColor=Color.DarkBlue, yConstPosition=Constraint.RelativeToParent((parent) => {return parent.Y + 7 + 30;}), xConstPosition=Constraint.RelativeToParent((parent) => {return parent.Width*0.525;})}
            };
        }

    }
}
