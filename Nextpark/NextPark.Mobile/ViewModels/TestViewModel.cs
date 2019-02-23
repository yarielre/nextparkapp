using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using NextPark.Enums.Enums;
using NextPark.Mobile.Services;
using NextPark.Mobile.Services.DataInterface;
using NextPark.Models;
using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
    public class TestViewModel : BaseViewModel
    {
        //fields
        private readonly IEventDataService _eventDataService;

        private readonly IParkingDataService _parkingDataService;
        private string _resultEventMonthly;
        private string _resultEventWeekly;

        public TestViewModel(IApiService apiService, IAuthService authService, INavigationService navService,
            IEventDataService eventDataService, IParkingDataService parkingDataService) : base(apiService, authService,
            navService)
        {
            _eventDataService = eventDataService;
            _parkingDataService = parkingDataService;
            CreateOrder();
        }

        //Commands
        public ICommand CreteEventWeeklyCommand => new Command(CreteEventWeeklyAsync);
        public ICommand CreteEventMonthtlyCommand => new Command(CreteEventMonthtlyAsync);
        public ICommand EditEventCommand => new Command(EditEventAsync().Wait);

        private async  Task EditEventAsync()
        {
            var ev = _eventDataService.GetEventAsync(1);    //check if event with id=1 exist
            if (ev!=null)
            {
                //TODO
            }
        }


        //Properties
        public string ResultEventWeekly
        {
            get => _resultEventWeekly;
            set => SetValue(ref _resultEventWeekly, value);
        }

        public string ResultEventMonthly
        {
            get => _resultEventMonthly;
            set => SetValue(ref _resultEventMonthly, value);
        }

        private async void CreteEventMonthtlyAsync()
        {
            var newEvent = new EventModel
            {
                StartDate = new DateTime(2019, 2, 1, 1, 0, 0), //start date 01/02/2019 01:00:00
                EndDate = new DateTime(2019, 2, 28, 3, 0, 0), //end date   28/02/2019 03:00:00
                ParkingId = 1,
                RepetitionEndDate = new DateTime(2019, 2, 28, 3, 0, 0),
                RepetitionType = RepetitionType.Monthly,
                MonthlyRepeatDay = new List<int>
                {
                    10,
                    15,
                    23
                }
            };

            var result = await _eventDataService.CreateEventAsync(newEvent).ConfigureAwait(false);
            if (result == null)
                ResultEventMonthly = "Failure";
            if (result != null)
                ResultEventMonthly = $"Success: {result.Count} items";
        }

        private async void CreteEventWeeklyAsync()
        {
            var newEvent = new EventModel
            {
                StartDate = new DateTime(2019, 2, 1, 1, 0, 0), //start date 01/02/2019 01:00:00
                EndDate = new DateTime(2019, 2, 28, 3, 0, 0), //end date   28/02/2019 03:00:00
                ParkingId = 1,
                RepetitionEndDate = new DateTime(2019, 2, 28, 3, 0, 0),
                RepetitionType = RepetitionType.Weekly,
                WeeklyRepeDayOfWeeks = new List<DayOfWeek>
                {
                    DayOfWeek.Monday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Friday
                }
            };

            //JObject o = (JObject)JToken.FromObject(newEvent);
            //Json = o.ToString();
            var result = await _eventDataService.CreateEventAsync(newEvent).ConfigureAwait(false);
            if (result == null)
                ResultEventWeekly = "Failure";
            if (result != null)
                ResultEventWeekly = $"Success: {result.Count} items";
        }

        #region Order

        private async Task CreateOrder()
        {
            var order = new OrderModel
            {
                StartDate = new DateTime(2019, 2, 1, 1, 0, 0), //start date 01/02/2019 01:00:00
                EndDate = new DateTime(2019, 2, 1, 2, 0, 0), //end date   28/02/2019 03:00:00
                ParkingId = 1,
                UserId = 1,
                Price = 20,
                PaymentCode = "drtrtrt"
            };
            JObject o = (JObject)JToken.FromObject(order);
            var jj = o.ToString();
        }
        #endregion
    }
}