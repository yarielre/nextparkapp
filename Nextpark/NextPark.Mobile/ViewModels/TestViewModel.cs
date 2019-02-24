using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using NextPark.Enums.Enums;
using NextPark.Mobile.Services;
using NextPark.Mobile.Services.Data;
using NextPark.Mobile.Settings;
using NextPark.Models;
using Xamarin.Forms;

namespace NextPark.Mobile.ViewModels
{
    public class TestViewModel : BaseViewModel
    {
        //fields
        private readonly IEventDataService _eventDataService;
        private readonly IParkingDataService _parkingDataService;

        private bool _startButtonEnabled;
        private string _resultConsole;
        private StringBuilder _consoleBuffer;

        public TestViewModel(IApiService apiService, IAuthService authService, INavigationService navService,
            IEventDataService eventDataService, IParkingDataService parkingDataService) : base(apiService, authService,
            navService)
        {
            _eventDataService = eventDataService;
            _parkingDataService = parkingDataService;

            CleanConsoleAsync();

        }


        //Properties
        public bool StartButtonEnabled
        {
            get => _startButtonEnabled;
            set => SetValue(ref _startButtonEnabled, value);
        }
        

        public string ResultConsole
        {
            get => _resultConsole;
            set => SetValue(ref _resultConsole, value);
        }

        //Commands
        public ICommand StartTesting => new Command(StartTestingAsync);
        public ICommand CleanConsole => new Command(CleanConsoleAsync);

        private async void StartTestingAsync()
        {
            StartButtonEnabled = false;

            await AuthServiceTest();
            await ParkingServiceTest();

            await LogoutTest();

            StartButtonEnabled = true;
        }

        private void CleanConsoleAsync()
        {
            _consoleBuffer = new StringBuilder();
            _consoleBuffer.AppendLine();
            _consoleBuffer.AppendLine();
            _consoleBuffer.AppendLine();
            _consoleBuffer.Append("STARTING SERVICES TESTING...");
            _consoleBuffer.AppendLine();
            _consoleBuffer.AppendLine();
            ResultConsole = _consoleBuffer.ToString();
        }

        private void AddLineToConsole(string data)
        {
            _consoleBuffer.AppendLine(data);
            ResultConsole = string.Empty;
            ResultConsole = _consoleBuffer.ToString();
        }


        #region Parking Test

        private async Task LogoutTest() {

            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING LOGOUT...");

            //Demo Login OK
            var loginResponse = await AuthService.Logout();
            if (loginResponse.IsSuccess)
            {
                AddLineToConsole("Logout OK");
            }
            else {
                AddLineToConsole("Logout FAILED");
            }
          
        }

        private async Task AuthServiceTest()
        {
            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING LOGIN...");

            //Demo Login OK
            var loginResponse = await AuthService.Login("info@nextpark.ch", "NextPark.1");

            if (loginResponse.IsSuccess)
            {
                AddLineToConsole("Login Ok");
                AddLineToConsole($"User id: {loginResponse.UserId}");
                AddLineToConsole($"User name: {loginResponse.UserName}");

                AddLineToConsole("Getting full user information...");

                var userData = await AuthService.GetUserByUserName(loginResponse.UserName);

                if (userData.IsSuccess)
                {
                    AddLineToConsole("Full user information OK");
                }
                else
                {
                    AddLineToConsole("Full user information FAILED");
                }
            }
            else
            {
                AddLineToConsole("Login FAILED");
            }

            AddLineToConsole("TESTING REGISTER...");
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


            if (registerResponse.IsSuccess)
            {
                AddLineToConsole("Register OK");
                AddLineToConsole($"User token: {registerResponse.AuthToken}");
                AddLineToConsole($"User id: {registerResponse.UserId}");
            }
            else
            {
                AddLineToConsole("Register FAILED");
            }
        }

        private async Task ParkingServiceTest()
        {
            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING PARKING SERVICE...");

            AddLineToConsole("Getting all parkings");
            var parkings = await _parkingDataService.GetAllParkingsAsync();

            AddLineToConsole($"Found: {parkings.Count} parkings");


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
                ImageUrl = $"{ApiSettings.BaseUrl}/images/image_parking1.png"
            };

            AddLineToConsole("Creating one parking");
            var postedParking = await _parkingDataService.CreateParkingAsync(parking1);

            if (postedParking != null)
            {
                AddLineToConsole("Creating the parking OK");
            }
            else
            {
                AddLineToConsole("Creating the parking FAILED");
            }
           
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

            AddLineToConsole("Editing the parking");
            var parkingResult = await _parkingDataService.EditParkingAsync(postedParking);

            if (parkingResult != null)
            {
                AddLineToConsole("Editing the parking OK");
            }
            else
            {
                AddLineToConsole("Editing the parking FAILED");
            }

            AddLineToConsole("Deleting the parking");
            var deletedParking = await _parkingDataService.DeleteParkingsAsync(parkingResult.Id);
            if (deletedParking != null)
            {
                AddLineToConsole("Deleting the parking OK");
            }
            else
            {
                AddLineToConsole("Deleting the parking FAILED");
            }

        }

        #endregion

        //#region Event Testing
        //private async Task EditEventAsync()
        //{
        //    var ev = _eventDataService.GetEventAsync(1);    //check if event with id=1 exist
        //    if (ev != null)
        //    {
        //        //TODO
        //    }
        //}

        //private async void CreteEventMonthtlyAsync()
        //{
        //    var newEvent = new EventModel
        //    {
        //        StartDate = new DateTime(2019, 2, 1, 1, 0, 0), //start date 01/02/2019 01:00:00
        //        EndDate = new DateTime(2019, 2, 28, 3, 0, 0), //end date   28/02/2019 03:00:00
        //        ParkingId = 1,
        //        RepetitionEndDate = new DateTime(2019, 2, 28, 3, 0, 0),
        //        RepetitionType = RepetitionType.Monthly,
        //        MonthlyRepeatDay = new List<int>
        //        {
        //            10,
        //            15,
        //            23
        //        }
        //    };

        //    var result = await _eventDataService.CreateEventAsync(newEvent).ConfigureAwait(false);
        //    if (result == null)
        //        AddLineToConsole("Failure");
        //    if (result != null)
        //        AddLineToConsole($"Success: {result.Count} items Failure");

        //}

        //private async void CreteEventWeeklyAsync()
        //{
        //    var newEvent = new EventModel
        //    {
        //        StartDate = new DateTime(2019, 2, 1, 1, 0, 0), //start date 01/02/2019 01:00:00
        //        EndDate = new DateTime(2019, 2, 28, 3, 0, 0), //end date   28/02/2019 03:00:00
        //        ParkingId = 1,
        //        RepetitionEndDate = new DateTime(2019, 2, 28, 3, 0, 0),
        //        RepetitionType = RepetitionType.Weekly,
        //        WeeklyRepeDayOfWeeks = new List<DayOfWeek>
        //        {
        //            DayOfWeek.Monday,
        //            DayOfWeek.Wednesday,
        //            DayOfWeek.Friday
        //        }
        //    };

        //    //JObject o = (JObject)JToken.FromObject(newEvent);
        //    //Json = o.ToString();
        //    var result = await _eventDataService.CreateEventAsync(newEvent).ConfigureAwait(false);
        //    if (result == null)
        //        AddLineToConsole("Failure");
        //    if (result != null)
        //        AddLineToConsole($"Success: {result.Count} items Failure");
        //}
        //#endregion


        //#region Order
        //private async Task CreateOrder()
        //{
        //    var order = new OrderModel
        //    {
        //        StartDate = new DateTime(2019, 2, 1, 1, 0, 0), //start date 01/02/2019 01:00:00
        //        EndDate = new DateTime(2019, 2, 1, 2, 0, 0), //end date   28/02/2019 03:00:00
        //        ParkingId = 1,
        //        UserId = 1,
        //        Price = 20,
        //        PaymentCode = "drtrtrt"
        //    };
        //}
        //#endregion


    }
}