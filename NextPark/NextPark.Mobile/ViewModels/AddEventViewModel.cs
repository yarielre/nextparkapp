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
using System.Collections.Generic;

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

        public string Title { get; set; }           // Page Title
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
                if (_startDate > EndDate) {
                    EndDate = StartDate;
                    base.OnPropertyChanged("EndDate");
                }
                RepetitionMinEndDate = StartDate;
                MinEndDate = StartDate;
                base.OnPropertyChanged("RepetitionMinEndDate");
                base.OnPropertyChanged("MinEndDate");
            } 
        }
        public DateTime MinStartDate { get; set; }  // Minimum event start date
        public DateTime MinEndDate { get; set; }  // Minimum event start date
        public DateTime EndDate { get; set; }       // Event end date
        public DateTime RepetitionEndDate { get; set; }
        public DateTime RepetitionMinEndDate { get; set; }
        public DateTime RepetitionMaxEndDate { get; set; }
        public bool RepetitionEndEnable { get; set; }
        public TimeSpan StartTime { get; set; }     // Event timeslot start
        public TimeSpan EndTime { get; set; }       // Event timeslot end
        public int RepetitionIndex                  // Repetition Selected Index
        {
            get { return _repetitionIndex; }
            set { _repetitionIndex = value; OnRepetitionChangedMethod(_repetitionIndex); } 
        }
        public bool RepetitionEnable { get; set; }  // Repetition Enable
        public bool EndDateVisible { get; set; }        // End date visibility
        public bool RepetitionEndVisible { get; set; }  // Repetition end visibility

        public bool TimeSlotVisible { get; set; }   // Timeslot visible
        public bool AddButtonVisible { get; set; }      // Add/Save button visible
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
        public bool WeekDayEnable { get; set; }
        public Color WeekDayBackgroundColor { get; set; }

        // PRIVATE VARIABLES
        private EventModel _event { get; set; }
        private bool _modifying { get; set; }
        private int _repetitionIndex { get; set; }

        // SERVICES
        private readonly IDialogService _dialogService;
        private readonly IEventDataService _eventDataService;
        private readonly IProfileService _profileService;

        // METHODS
        public AddEventViewModel(IDialogService dialogService,
                                 IApiService apiService,
                                 IAuthService authService,
                                 INavigationService navService,
                                 IEventDataService eventDataService,
                                 IProfileService profileService)
                                 : base(apiService, authService, navService)
        {
            _dialogService = dialogService;
            _eventDataService = eventDataService;
            _profileService = profileService;

            UserName = AuthSettings.User.Name;
            UserMoney = AuthSettings.UserCoin.ToString("N2");
            base.OnPropertyChanged("UserName");
            base.OnPropertyChanged("UserMoney");

            // Header actions
            OnBackClick = new Command<object>(OnBackClickMethod);
            OnUserClick = new Command<object>(OnUserClickMethod);
            OnMoneyClick = new Command<object>(OnMoneyClickMethod);
            OnAddClick = new Command<object>(OnAddClickMethod);
            OnDeleteClick = new Command<object>(OnDeleteClickMethod);

            TimeSlotVisible = false;
            EndDateVisible = true;

            MinStartDate = DateTime.Now;
            RepetitionMinEndDate = DateTime.Now.Date;
            RepetitionMaxEndDate = DateTime.Now.AddYears(1).Date;

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
                    UserMoney = AuthSettings.UserCoin.ToString("N2");
                    base.OnPropertyChanged("BackText");
                    base.OnPropertyChanged("UserName");
                    base.OnPropertyChanged("UserMoney");

                    if (_event.Id == 0) {
                        // New Event
                        Title = "Aggiungi disponibilità";
                        AllDaySwitchToggled = true;
                        AllDayTextColor = Color.Black;

                        if (_event.StartDate < DateTime.Now.Date)
                        {
                            StartDate = DateTime.Now.Date;
                        } else {
                            StartDate = _event.StartDate.Date;
                        }
                        StartTime = DateTime.Now.TimeOfDay;
                        EndDate = DateTime.Now.Date;
                        EndTime = DateTime.Now.TimeOfDay;
                        RepetitionEndDate = StartDate;
                        _event.RepetitionType = RepetitionType.None;
                        _event.WeeklyRepeDayOfWeeks = new List<DayOfWeek>
                        {
                            (DayOfWeek)(DateTime.Now.DayOfWeek)
                        };
                        RepetitionIndex = 0;
                        AddButtonVisible = true;
                        DeleteButtonVisible = false;
                        RepetitionEnable = true;
                        RepetitionEndEnable = true;
                        WeekDayEnable = true;
                        WeekDayBackgroundColor = Color.White;
                        AddButtonText = "Aggiungi";
                        _modifying = false;

                    } else {
                        // Modify an Event
                        Title = "Dettaglio disponibilità";
                        StartDate = _event.StartDate.Date;
                        StartTime = _event.StartDate.TimeOfDay;
                        EndDate = _event.EndDate.Date;
                        EndTime = _event.EndDate.TimeOfDay;
                        RepetitionEndDate = _event.RepetitionEndDate;
                        if (_event.WeeklyRepeDayOfWeeks == null) {
                            _event.WeeklyRepeDayOfWeeks = new List<DayOfWeek>
                            {
                                (DayOfWeek)(DateTime.Now.DayOfWeek)
                            };
                        }
                        if (EndDate >= DateTime.Now.Date)
                        {
                            AddButtonVisible = true;
                            DeleteButtonVisible = true;
                            MinStartDate = DateTime.Now;
                        } else {
                            AddButtonVisible = false;
                            DeleteButtonVisible = false;
                            MinStartDate = _event.StartDate.Date;
                        }
                        RepetitionEnable = false;
                        RepetitionEndEnable = false;
                        WeekDayEnable = false;
                        WeekDayBackgroundColor = Color.FromHex("#FAFAFA");

                        AddButtonText = "Modifica";
                        _modifying = true;
                    }

                    RepetitionMinEndDate = StartDate;
                    switch (_event.RepetitionType) 
                    {
                        default:
                        case RepetitionType.None:
                            RepetitionIndex = 0;
                            RepetitionEndVisible = false;
                            WeekDayVisible = false;
                            EndDateVisible = true;
                            break;
                        case RepetitionType.Dayly:
                            RepetitionIndex = 1;
                            RepetitionEndVisible = true;
                            WeekDayVisible = false;
                            EndDateVisible = false;
                            break;
                        case RepetitionType.Weekly:
                            RepetitionIndex = 2;
                            RepetitionEndVisible = true;
                            WeekDayVisible = true;
                            EndDateVisible = false;
                            break;
                    }

                    base.OnPropertyChanged("Title");
                    base.OnPropertyChanged("AllDaySwitchToggled");
                    base.OnPropertyChanged("AllDayTextColor");
                    base.OnPropertyChanged("MinStartDate");
                    base.OnPropertyChanged("StartDate");
                    base.OnPropertyChanged("StartTime");
                    base.OnPropertyChanged("EndDate");
                    base.OnPropertyChanged("EndDateVisible");
                    base.OnPropertyChanged("EndTime");
                    base.OnPropertyChanged("RepetitionEnable");
                    base.OnPropertyChanged("RepetitionEndEnable");
                    base.OnPropertyChanged("RepetitionEndVisible");
                    base.OnPropertyChanged("RepetitionIndex");
                    base.OnPropertyChanged("RepetitionMinEndDate");
                    base.OnPropertyChanged("RepetitionEndDate");
                    base.OnPropertyChanged("WeekDayEnable");
                    base.OnPropertyChanged("WeekDayBackgroundColor");
                    base.OnPropertyChanged("WeekDaysVisible");

                    base.OnPropertyChanged("AddButtonVisible");
                    base.OnPropertyChanged("DeleteButtonVisible");
                    base.OnPropertyChanged("AddButtonText");

                    WeekDays.Add(new UISelectionItem { Text = "Lunedì", Selected = false, Id = 1});
                    WeekDays.Add(new UISelectionItem { Text = "Martedì", Selected = false, Id = 2});
                    WeekDays.Add(new UISelectionItem { Text = "Mercoledì", Selected = false, Id = 3 });
                    WeekDays.Add(new UISelectionItem { Text = "Giovedì", Selected = false, Id = 4 });
                    WeekDays.Add(new UISelectionItem { Text = "Venerdì", Selected = false, Id = 5 });
                    WeekDays.Add(new UISelectionItem { Text = "Sabato", Selected = false, Id = 6 });
                    WeekDays.Add(new UISelectionItem { Text = "Domenica", Selected = false, Id = 0 });

                    foreach (DayOfWeek dayOfWeek in _event.WeeklyRepeDayOfWeeks) {
                        int index = ((int)dayOfWeek + 6) % 7;
                        WeekDays[index].Selected = true;
                    }
                }
            }
            return Task.FromResult(false);
        }

        public override bool BackButtonPressed()
        {
            OnBackClickMethod(null);
            return false; // Do not propagate back button pressed
        }

        // Back Click action
        public void OnBackClickMethod(object sender)
        {
            //NavigationService.NavigateToAsync<UserParkingViewModel>();
            NavigationService.NavigateToAsync<ParkingDataViewModel>(_profileService.LastEditingParking);
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
            // Show activity spinner
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");

            // Check for orders
            if (_modifying)
            {
                // Remove seconds from event
                StartTime = StartTime.Subtract(TimeSpan.FromSeconds(StartTime.Seconds));
                EndTime = EndTime.Subtract(TimeSpan.FromSeconds(EndTime.Seconds));
                DateTime start = StartDate + StartTime;
                DateTime end = EndDate + EndTime;

                UIParkingModel parking = _profileService.GetParkingById(_event.ParkingId);
                /*
                foreach (OrderModel order in parking.Orders)
                {
                    if ((order.StartDate < _event.EndDate) && (order.EndDate > _event.StartDate))
                    {
                        if (((StartTime > TimeSpan.FromMinutes(0)) && (start > order.StartDate)) || ((EndTime < TimeSpan.FromMinutes(1439)) && (end < order.EndDate)))
                        {
                            // Orders are present, unauthorized to modify event
                            _dialogService.ShowAlert("Errore", "Non è stato possibile modificare la disponibilità in quanto ci sono degli ordini confermati all'interno di queste date.");
                            IsRunning = false;
                            base.OnPropertyChanged("IsRunning");
                            return;
                        }
                    }
                }
                */
            }
            // Try to add or modify events
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
                    EndDateVisible = true;
                    break;
                case 1: // Daily
                    RepetitionEndVisible = true;
                    WeekDayVisible = false;
                    EndDateVisible = false;
                    break;
                case 2: // Weekly
                    RepetitionEndVisible = true;
                    WeekDayVisible = true;
                    EndDateVisible = false;
                    break;
            }
            base.OnPropertyChanged("RepetitionEndVisible");
            base.OnPropertyChanged("WeekDayVisible");
            base.OnPropertyChanged("EndDateVisible");
        }

        // Week Day Selected
        public void OnWeekDaySelected(object item)
        {
            UISelectionItem selItem = item as UISelectionItem;
            if (selItem.Selected == true) {
                selItem.Selected = false;
                _event.WeeklyRepeDayOfWeeks.Remove((DayOfWeek)selItem.Id);
            } else {
                selItem.Selected = true;
                _event.WeeklyRepeDayOfWeeks.Add((DayOfWeek)selItem.Id);
            }
            base.OnPropertyChanged("WeekDays");
        }

        // Delete event button action
        public void OnDeleteClickMethod(object sender)
        {
            // Show activty spinner
            IsRunning = true;
            base.OnPropertyChanged("IsRunning");

            // Check for orders
            UIParkingModel parking = _profileService.GetParkingById(_event.ParkingId);
            foreach (OrderModel order in parking.Orders) {
                if ((order.StartDate < _event.EndDate) && (order.EndDate > _event.StartDate)) {
                    // Orders are present, unauthorized to delete event
                    _dialogService.ShowAlert("Errore", "Non è possibile eliminare la disponibilità in quanto ci sono degli ordini confermati all'interno di queste date.");
                    IsRunning = false;
                    base.OnPropertyChanged("IsRunning");
                    return;
                }
            }
            // Try to delete event
            DeleteEvent();
        }

        public async void AddEvent()
        {
            // Check Data
            if (((RepetitionIndex != 0) || (StartDate == EndDate)) && (StartTime > EndTime)) {
                await _dialogService.ShowAlert("Errore", "L'orario di fine deve essere maggiore a quello di inizio");
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
                return;
            }

            switch (RepetitionIndex)
            {
                default:
                case 0: // Never
                    _event.RepetitionType = RepetitionType.None;
                    RepetitionEndDate = EndDate;
                    break;
                case 1: // Daily
                    _event.RepetitionType = RepetitionType.Dayly;
                    EndDate = StartDate;
                    break;
                case 2: // Weekly
                    _event.RepetitionType = RepetitionType.Weekly;
                    EndDate = StartDate;
                    break;
            }
            // Remove seconds from timing
            StartTime = StartTime.Subtract(TimeSpan.FromSeconds(StartTime.Seconds));
            EndTime = EndTime.Subtract(TimeSpan.FromSeconds(EndTime.Seconds));

            _event.StartDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Hours, StartTime.Minutes, 0);
            _event.EndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, EndTime.Hours, EndTime.Minutes, 0);
            _event.RepetitionEndDate = new DateTime(RepetitionEndDate.Year, RepetitionEndDate.Month, RepetitionEndDate.Day, EndTime.Hours, EndTime.Minutes, 0);

            var result = new List<EventModel>();

            try
            {
                if (_modifying)
                {

                    var choice = "Modifica solo questo evento";

                    // TODO: check for repetition
                    if (_event.RepetitionType != RepetitionType.None)
                    {
                        // Show choice popup
                        choice = await Application.Current.MainPage.DisplayActionSheet(
                                        "Questo è un evento periodico",
                                        "Annulla",
                                        null,
                                        "Modifica solo questo evento",
                                        "Modifica tutti gli eventi futuri");
                        if ((choice == null) || (choice.Equals("Annulla")))
                        {
                            IsRunning = false;
                            base.OnPropertyChanged("IsRunning");
                            return;
                        }
                    }

                    if (choice.Equals("Modifica solo questo evento"))
                    {
                        // TODO: remove repetition
                        // _event.RepetitionType = none
                        // _event.RepetitionId = ?
                        var resultEdit = await _eventDataService.EditEventsAsync(_event.Id, _event);

                        if (resultEdit != null) {
                            if (resultEdit.IsSuccess == true) {
                                result.Add(resultEdit.Result);
                            } else if (resultEdit.ErrorType == ErrorType.EventCantBeModified) {
                                await _dialogService.ShowAlert("Errore", "Non è possibile modificare la disponibilità in quanto ci sono degli ordini confermati all'interno di queste date.");
                            } else {
                                await _dialogService.ShowAlert("Errore", "Non è possibile modificare la disponibilità.");
                            }
                        } else {
                            await _dialogService.ShowAlert("Errore", "Non è possibile modificare la disponibilità.");
                        }
                    }
                    else if (choice.Equals("Modifica tutti gli eventi futuri"))
                    {
                        var resultEdit = await _eventDataService.EditSerieEventsAsync(_event);
                        if (resultEdit != null)
                        {
                            if (resultEdit.IsSuccess == true)
                            {
                                result = resultEdit.Result;
                            }
                            else if (resultEdit.ErrorType == ErrorType.EventCantBeModified)
                            {
                                await _dialogService.ShowAlert("Errore", "Non è possibile modificare tutte le disponibilità in quanto ci sono degli ordini confermati all'interno di queste date.");
                            }
                            else
                            {
                                await _dialogService.ShowAlert("Errore", "Non è possibile modificare le disponibilità.");
                            }
                        } else {
                            await _dialogService.ShowAlert("Errore", "Non è possibile modificare le disponibilità.");
                        }
                    }
                }
                else
                {
                    result = await _eventDataService.CreateEventAsync(_event);
                }

                // Refresh parking events
                // TODO: improve with getEventByParkingId
                var eventsResult = await _eventDataService.GetAllEventsAsync();
                if ((eventsResult != null) && (eventsResult.Count != 0))
                {
                    UIParkingModel uiParking = _profileService.GetParkingById(_event.ParkingId);
                    uiParking.Events.Clear();
                    foreach (EventModel availability in eventsResult)
                    {
                        if (availability.ParkingId == _event.ParkingId) {
                            uiParking.Events.Add(availability);
                        }
                    }
                }
            }
            catch (Exception e) { }
            finally
            {
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");

                if ((result != null) && (result.Count > 0))
                {
                    await NavigationService.NavigateToAsync<ParkingDataViewModel>(_profileService.LastEditingParking);
                }
            }
        }

        public async void DeleteEvent()
        {
            var choice = "Elimina solo questo evento";

            // TODO: check for repetition
            if (_event.RepetitionType != RepetitionType.None)
            {
                // Show choice popup
                choice = await Application.Current.MainPage.DisplayActionSheet(
                                "Questo è un evento periodico",
                                "Annulla",
                                null,
                                "Elimina solo questo evento",
                                "Elimina tutti gli eventi futuri");
                if ((choice == null) || (choice.Equals("Annulla")))
                {
                    IsRunning = false;
                    base.OnPropertyChanged("IsRunning");
                    return;
                }
            }

            // Delete event or event serie
            try
            {
                if (choice.Equals("Elimina solo questo evento"))
                {
                    var result = await _eventDataService.DeleteEventsAsync(_event.Id);

                    if (result != null) {
                        if (result.IsSuccess == true) {

                        } else if (result.ErrorType == ErrorType.EventCantBeModified) {
                            await _dialogService.ShowAlert("Errore", "Non è possibile eliminare la disponibilità in quanto ci sono degli ordini confermati all'interno di queste date.");
                        } else {
                            await _dialogService.ShowAlert("Errore", "Non è possibile eliminare la disponibilità.");
                        }
                    } else {
                        await _dialogService.ShowAlert("Errore", "Non è possibile eliminare la disponibilità.");
                    }
                }
                else if (choice.Equals("Elimina tutti gli eventi futuri"))
                {
                    // TODO: verify GetHash or add DeleteSerieEventsAsync(Guid Id)
                    var result = await _eventDataService.DeleteSerieEventsAsync(_event.Id);
                    if (result != null)
                    {
                        if (result.IsSuccess == true)
                        {

                        }
                        else if (result.ErrorType == ErrorType.EventCantBeModified)
                        {
                            await _dialogService.ShowAlert("Errore", "Non è possibile eliminare le disponibilità in quanto ci sono degli ordini confermati all'interno di queste date.");
                        }
                        else
                        {
                            await _dialogService.ShowAlert("Errore", "Non è possibile eliminare le disponibilità.");
                        }
                    }
                    else
                    {
                        await _dialogService.ShowAlert("Errore", "Non è possibile eliminare le disponibilità.");
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                IsRunning = false;
                base.OnPropertyChanged("IsRunning");
                await NavigationService.NavigateToAsync<ParkingDataViewModel>(_profileService.LastEditingParking);
            }        
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
