using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Services;
using NextPark.Enums;
using NextPark.Models;
using Xamarin.Forms;

namespace Inside.Xamarin.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private TimeSpan _endTime;
        private TimeSpan _startTime;
        
        private bool _isRepeated;
       // private ObservableCollection<MyMonthOfYear> _monthRepeat;
       // private ObservableCollection<MyDayOfWeek> _weekRepeat;
        private ObservableCollection<SelectableItem<MyDayOfWeek>> _myDayOfWeeksRepeat;

        private DateTime _endDate;
        private DateTime _startDate;

        public EventViewModel(bool isByHours)
        {
            _navigationService = NavigationService.GetInstance();
            ParkingEvent = new EventModel
            {
                WeekRepeat = new List<MyDayOfWeek>(),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                StartTime = DateTime.Now.TimeOfDay,
                EndTime = DateTime.Now.TimeOfDay
            };
            IsByHours = isByHours;
            IsByMonth = !isByHours;
            OnInit();
            //MessagingCenter.Subscribe<List<SelectableItem<MyDayOfWeek>>>(this, Messages.DayRepeatSent, (list) =>
            //{
            //    foreach (var selectableItem in list)
            //    {
            //        ParkingEvent.WeekRepeat.Add(selectableItem.Data);
            //    }
            //});
        }

        public EventViewModel(EventModel parkingEvent, bool isByHours)
        {
            IsByHours = isByHours;
            IsByMonth = !isByHours;
            _navigationService = NavigationService.GetInstance();
            ParkingEvent = parkingEvent;
            OnInit();
            if (parkingEvent.WeekRepeat.Count > 0)
            {
                foreach (var myDayOfWeek in parkingEvent.WeekRepeat)
                {
                    SelectableItem<MyDayOfWeek> item =
                        this.MyDayOfWeeksRepeat.FirstOrDefault(day => day.Data == myDayOfWeek);
                    if (item != null)
                    {
                        item.IsSelected = true;
                    }
                }
            }
        }

        //public ObservableCollection<MyDayOfWeek> WeekRepeat
        //{
        //    get => _weekRepeat;
        //    set => SetValue(ref _weekRepeat, value);
        //}
        public ObservableCollection<SelectableItem<MyDayOfWeek>> MyDayOfWeeksRepeat
        {
            get => _myDayOfWeeksRepeat;
            set => SetValue(ref _myDayOfWeeksRepeat, value);
        }

        //public ObservableCollection<MyMonthOfYear> MonthRepeat
        //{
        //    get => _monthRepeat;
        //    set => SetValue(ref _monthRepeat, value);
        //}
        public bool IsRepeated
        {
            get => _isRepeated;
            set => SetValue(ref _isRepeated, value);
        }
        public TimeSpan StartTime
        {
            get => _startTime;
            set => SetValue(ref _startTime, value);
        }
        public TimeSpan EndTime
        {
            get => _endTime;
            set => SetValue(ref _endTime, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetValue(ref _endDate, value);
        }
        public DateTime StartDate
        {
            get => _startDate;
            set => SetValue(ref _startDate, value);
        }
        public EventModel ParkingEvent { get; set; }

       
        public bool IsByHours { get; set; }
        public bool IsByMonth { get; set; }

        public ICommand EventCreateCommand => new RelayCommand(EventCreate);
     //   public ICommand EventRepeatCommand => new RelayCommand(EventRepeat);

        private async void EventRepeat()
        {
       //     MainViewModel.GetInstance().DayRepeat = new DayRepeatViewModel(ParkingEvent.WeekRepeat);
        //    await NavigationService.GetInstance().NavigateOnMaster(Pages.DayRepeatView);
        }
        private void OnInit()
        {
            this.MyDayOfWeeksRepeat = new ObservableCollection<SelectableItem<MyDayOfWeek>>();

            MyDayOfWeeksRepeat.Add(new SelectableItem<MyDayOfWeek>(MyDayOfWeek.Sunday, false));
            MyDayOfWeeksRepeat.Add(new SelectableItem<MyDayOfWeek>(MyDayOfWeek.Monday, false));
            MyDayOfWeeksRepeat.Add(new SelectableItem<MyDayOfWeek>(MyDayOfWeek.Tuesday, false));
            MyDayOfWeeksRepeat.Add(new SelectableItem<MyDayOfWeek>(MyDayOfWeek.Wednesday, false));
            MyDayOfWeeksRepeat.Add(new SelectableItem<MyDayOfWeek>(MyDayOfWeek.Thursday, false));
            MyDayOfWeeksRepeat.Add(new SelectableItem<MyDayOfWeek>(MyDayOfWeek.Friday, false));
            MyDayOfWeeksRepeat.Add(new SelectableItem<MyDayOfWeek>(MyDayOfWeek.Saturday, false));
        }
        private async void EventCreate()
        {
            var itemSelected = this.MyDayOfWeeksRepeat.Where(day => day.IsSelected).ToList();
            ParkingEvent.WeekRepeat = new List<MyDayOfWeek>();
            foreach (var selectableItem in itemSelected)
                    {
                        ParkingEvent.WeekRepeat.Add(selectableItem.Data);
                    }
                MessagingCenter.Send(ParkingEvent, Messages.ParkingEventCreated);
            await _navigationService.BackOnMaster();
        }
    }
}