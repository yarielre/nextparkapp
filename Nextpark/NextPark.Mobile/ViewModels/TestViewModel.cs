using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using NextPark.Enums.Enums;
using NextPark.Mobile.Services;
using NextPark.Mobile.Services.Data;
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

        #region Parking Test

        private async void DemoBackEndCalls()
        {
            /*
            try
            {
                //Demo Login OK
                var loginResponse = await AuthService.Login("demo@nextpark.ch", "Wisegar.1");

                //Demo Register OK
                var demoUser = new RegisterModel
                {
                    Address = "Via Demo User",
                    CarPlate = "TI 00DEMO00",
                    Email = "demo@nextpark.ch",
                    Lastname = "Demo",
                    Name = "User",
                    Password = "Wisegar.1",
                    State = "DemoState",
                    UserName = "demo@nextpark.ch"
                };

                var registerResponse = await AuthService.Register(demoUser);

                //Demo Get Parkings OK
                var parkings = await _parkingDataService.GetAllParkingsAsync();


                if (parkings.Count > 0) {
                    await _parkingDataService.DeleteParkingsAsync(parkings[0].Id);
                }

                //Demo Posting Parking
                var parking1 = new ParkingModel
                {
                    Address = "Via Strada",
                    Cap = 7777,
                    City = "Lugano",
                    CarPlate = "TI 000000",
                    Latitude = 40,
                    Longitude = 40,
                    PriceMax = 4,
                    PriceMin = 4,
                    State = "Ticino",
                    Status = Enums.Enums.ParkingStatus.Enabled,
                    UserId = 1,
                    ImageUrl = "image_parking1.png"
                };

                //Demo Posting Parking
                var postedParking = await _parkingDataService.CreateParkingAsync(parking1);

                var eventParking = new EventModel
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    ParkingId = postedParking.Id,
                    RepetitionEndDate = DateTime.Now,
                    RepetitionType = Enums.Enums.RepetitionType.Dayly
                };
                postedParking.Status = Enums.Enums.ParkingStatus.Disabled;

                var result = await _eventDataService.CreateEventAsync(eventParking);

                //Demo Puting Parking
                var parkingResult = await _parkingDataService.EditParkingAsync(postedParking);

                //Demo Deleting Parking
                var deletedParking = await _parkingDataService.DeleteParkingsAsync(parkingResult.Id);

            }
            catch (Exception e)
            {

            }
            */
        }

        #endregion
    }
}