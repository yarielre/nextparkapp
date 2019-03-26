using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using NextPark.Enums.Enums;
using NextPark.Mobile.Helpers;
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
        private readonly IOrderDataService _orderDataService;
        private readonly IPurchaseDataService _purchaseDataService;
        private readonly ILocalizationService _localizationService;

        private bool _startButtonEnabled;
        private string _resultConsole;
        private StringBuilder _consoleBuffer;

        public UserModel LoggedUser { get; set; }

        public TestViewModel(IApiService apiService, IAuthService authService, INavigationService navService,
            IEventDataService eventDataService, IParkingDataService parkingDataService, IOrderDataService orderDataService, IPurchaseDataService purchaseDataService, ILocalizationService localizationService) : base(apiService, authService,
            navService)
        {
            _eventDataService = eventDataService;
            _parkingDataService = parkingDataService;
            _orderDataService = orderDataService;
            _purchaseDataService = purchaseDataService;
            _localizationService = localizationService;

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
        public ICommand StartTesting => new Command(async ()=>
        {
            await StartTestingAsync().ConfigureAwait(false);
        });
        public ICommand CleanConsole => new Command(CleanConsoleAsync);

        private async Task StartTestingAsync()
        {
            StartButtonEnabled = false;
            
            await AuthServiceTest().ConfigureAwait(false);

            await ParkingServiceTest().ConfigureAwait(false);
            await OrderServiceTest().ConfigureAwait(false);
            await EventServiceTest().ConfigureAwait(false);
           // PurchaseServiceTest();
            await LogoutTest().ConfigureAwait(false);
           // LocalizationTest();
            #region Specific Cases
            await CaseEventDayly().ConfigureAwait(false); 
            #endregion
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

        #region Auth Testing
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
                    this.LoggedUser = userData.Result as UserModel;
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
                UserName = "demo@nextpark.ch",
                Phone = "00410000000",
                Cap = 11111,
                City = "Vezia"
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
        #endregion

        #region Parking Test




        private async Task ParkingServiceTest()
        {
            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING PARKING SERVICE...");

            AddLineToConsole("Getting all parkings");
            var parkings = await _parkingDataService.GetAllParkingsAsync().ConfigureAwait(false);
            AddLineToConsole($"Found: {parkings.Count} parkings");

            /////////////////////////////////////////////////////////////////////////Remove this
            //AddLineToConsole("Getting all events");
            //var events = await _eventDataService.GetAllEventsAsync();
            //AddLineToConsole($"Found: {events.Count} parkings");
            //if (events!=null && events.Count>0 )
            //{
            //    var count = 0;
            //    foreach (var item in events)
            //    {
            //        count++;
            //        AddLineToConsole($"---Event {count}-> Start date:{item.StartDate:G}  End date:{item.EndDate:G}");
            //    }
            //}
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
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
            var endDate = DateTime.Now.AddDays(1);
            var eventParking = new EventModel
            {
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                EndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 00),
                ParkingId = postedParking.Id,
                RepetitionEndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 00),
                RepetitionType = RepetitionType.Dayly
            };

            // postedParking.Status = Enums.Enums.ParkingStatus.Disabled;
            AddLineToConsole("----------Creating dayly events----------");
            AddLineToConsole($"->Start date:{eventParking.StartDate:G}");
            AddLineToConsole($"->End date:{eventParking.EndDate:G}");
            var result = await _eventDataService.CreateEventAsync(eventParking);
            if (result != null)
            {
                AddLineToConsole($"Created: {result.Count} dayly events for the created parking");
                AddLineToConsole("Creating dayly events OK");
            }
            else
            {
                AddLineToConsole("Creating dayly events FAILED");
            }

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

        #region Event Testing
        private async Task EventServiceTest()
        {

            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING EVENTS...");

            AddLineToConsole("Selecting one parking");
            var parkings = await _parkingDataService.GetAllParkingsAsync();

            AddLineToConsole($"Found: {parkings.Count} parkings");

            var selectedPArking = parkings.FirstOrDefault();
            if (selectedPArking != null)
            {
                AddLineToConsole("Selecting one parking OK");
            }
            else
            {
                AddLineToConsole("Selecting one parking FAILED");
                AddLineToConsole("Events testing FAILED");
                return;
            }

            AddLineToConsole("Creating monthly event for the selected parking");

            var newMonthlyEvent = new EventModel
            {
                StartDate = DateTime.Now,
                EndDate = (DateTime.Now).AddDays(3),
                ParkingId = selectedPArking.Id,
                RepetitionEndDate = new DateTime(2019, 2, 28, 3, 0, 0),
                RepetitionType = RepetitionType.Monthly,
                MonthlyRepeatDay = new List<int>
                {
                    10,
                    15,
                    23
                }
            };

            var createdMonthlyEvents = await _eventDataService.CreateEventAsync(newMonthlyEvent);

            if (createdMonthlyEvents != null)
            {
                AddLineToConsole($"Created: {createdMonthlyEvents.Count} monthly events for the selected parking");
                AddLineToConsole("Creating monthly events OK");
            }
            else
            {
                AddLineToConsole("Creating monthly events FAILED");
            }

            AddLineToConsole("Creating weekly event for the selected parking");

            var newWeklyEvent = new EventModel
            {
                StartDate = DateTime.Now,
                EndDate = (DateTime.Now).AddDays(4),
                ParkingId = selectedPArking.Id,
                RepetitionEndDate = (DateTime.Now).AddDays(4),
                RepetitionType = RepetitionType.Weekly,
                WeeklyRepeDayOfWeeks = new List<DayOfWeek>
                {
                    DayOfWeek.Monday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Friday
                }
            };

            var createdWeeklyEvents = await _eventDataService.CreateEventAsync(newWeklyEvent);

            if (createdWeeklyEvents != null)
            {
                AddLineToConsole($"Created: {createdWeeklyEvents.Count} weekly events for the selected parking");
                AddLineToConsole("Creating weekly events OK");
            }
            else
            {
                AddLineToConsole("Creating newWeklyEvent events FAILED");
            }

            AddLineToConsole("Creating dayly event for the selected parking");

            var newDaylyEvent = new EventModel
            {
                StartDate = DateTime.Now,
                EndDate = (DateTime.Now).AddDays(4),
                ParkingId = selectedPArking.Id,
                RepetitionEndDate = (DateTime.Now).AddDays(4),
                RepetitionType = RepetitionType.Dayly,
            };

            var createdDaylyEvents = await _eventDataService.CreateEventAsync(newDaylyEvent);

            if (createdDaylyEvents != null)
            {
                AddLineToConsole($"Created: {createdWeeklyEvents.Count} dayly events for the selected parking");
                AddLineToConsole("Creating dayly events OK");
            }
            else
            {
                AddLineToConsole("Creating dayly events FAILED");
            }

            AddLineToConsole("Getting all events of the selected parking");
            var parkingEvents = await _parkingDataService.GetParkingEventsAsync(selectedPArking.Id);

            if (parkingEvents != null)
            {
                AddLineToConsole($"Fount: {parkingEvents.Count} events for the selected parking");
                AddLineToConsole("Getting parking events OK");
            }
            else
            {
                AddLineToConsole("Getting parking events FAILED");
            }

            var firstRepitedEvent = parkingEvents.FirstOrDefault(e => e.RepetitionId != Guid.Empty);
            if (firstRepitedEvent != null)
            {
                AddLineToConsole("Editing all events of the selected parking by series");
                AddLineToConsole("Editing all events repetition end date to more 5 days");
                firstRepitedEvent.RepetitionEndDate = firstRepitedEvent.RepetitionEndDate.AddDays(5);

                var editedEvents = await _eventDataService.EditSerieEventsAsync(firstRepitedEvent);

                if (editedEvents != null)
                {

                    AddLineToConsole("Editing parking events by series OK");
                }
                else
                {
                    AddLineToConsole("Editing parking events by series FAILED");
                }

                AddLineToConsole("Deleting all events of the selected parking  by series ");
                var deletedSeriesParkingEvents = await _eventDataService.DeleteSerieEventsAsync(firstRepitedEvent.Id);

                if (deletedSeriesParkingEvents != null)
                {
                    AddLineToConsole($"Deleted: {deletedSeriesParkingEvents.Count} events for the selected parking  by series");

                    AddLineToConsole("Getting parking events  by series OK");
                }
                else
                {
                    AddLineToConsole("Getting parking events by series FAILED");
                }

            }

            AddLineToConsole("Deleting all events of the selected parking");
            var deletedParkingEvents = await _parkingDataService.DeleteParkingsEventsAsync(selectedPArking.Id);

            if (deletedParkingEvents != null)
            {
                AddLineToConsole($"Deleted: {deletedParkingEvents.Count} events for the selected parking");

                AddLineToConsole("Getting parking events OK");
            }
            else
            {
                AddLineToConsole("Getting parking events FAILED");
            }

        }
        #endregion

        #region Order Service
        private async Task OrderServiceTest()
        {

            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING ORDER SERVICE...");


            AddLineToConsole("Selecting one parking");
            var parkings = await _parkingDataService.GetAllParkingsAsync();

            AddLineToConsole($"Found: {parkings.Count} parkings");

            var selectedPArking = parkings.FirstOrDefault();
            if (selectedPArking != null)
            {
                AddLineToConsole("Selecting one parking OK");
            }
            else
            {
                AddLineToConsole("Selecting one parking FAILED");
                AddLineToConsole("Orders testing FAILED");
                return;
            }

            var startDate = DateTime.Now.ToUniversalTime();
            var endDate = startDate.AddHours(3);


            var newWeklyEvent = new EventModel
            {
                StartDate = startDate,
                EndDate = endDate,
                ParkingId = selectedPArking.Id,
                RepetitionEndDate = endDate.AddDays(30),
                RepetitionType = RepetitionType.Weekly,
                WeeklyRepeDayOfWeeks = new List<DayOfWeek>
                {
                    DayOfWeek.Monday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday,
                    DayOfWeek.Sunday
                }
            };

            var createdWeeklyEvents = await _eventDataService.CreateEventAsync(newWeklyEvent);
            if (createdWeeklyEvents != null)
            {
                AddLineToConsole($"Created: {createdWeeklyEvents.Count} weekly events for the selected parking");
                AddLineToConsole("Creating weekly events OK");
            }
            else
            {
                AddLineToConsole("Creating newWeklyEvent events FAILED");
                return;
            }

            var orderStartDate = DateTime.Now.ToUniversalTime();
            var orderEndDate = orderStartDate.AddHours(1);

            var order = new OrderModel
            {
                StartDate = orderStartDate,
                EndDate = orderEndDate,
                ParkingId = selectedPArking.Id,
                UserId = 1,
                Price = 10,
                PaymentCode = "drtrtrt",
                OrderStatus = Enums.OrderStatus.Actived,
                PaymentStatus = PaymentStatus.Pending
            };

            AddLineToConsole("Creating one order...");
            var postedOrderResponse = await _orderDataService.CreateOrderAsync(order).ConfigureAwait(false);

            AddLineToConsole(postedOrderResponse.IsSuccess ? $"Creating the order OK Msg: {postedOrderResponse.Message}" : $"Creating the order Failed Msg {postedOrderResponse.Message}");

            if (postedOrderResponse.Result != null)
            {
                AddLineToConsole("Editing the order...");
                postedOrderResponse.Result.PaymentStatus = PaymentStatus.Cancel;
                var editedOrder = _orderDataService.EditOrderAsync(postedOrderResponse.Result.Id, postedOrderResponse.Result);
                AddLineToConsole(editedOrder != null ? "Editing the order OK" : "Editing the order FAILED");
                AddLineToConsole("Terminate the order...");
                var finishedOrder = _orderDataService.TerminateOrderAsync(postedOrderResponse.Result.Id);
                AddLineToConsole(finishedOrder != null ? "Terminate the order OK" : "Terminate the order FAILED");
            }
        }
        #endregion

        #region Purchase Service
        private void PurchaseServiceTest()
        {
            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING PURCHASE SERVICE...");

            AddLineToConsole("Adding money...");
            var purchaseAddBalanceModel = _purchaseDataService.BuyAmount(new PurchaseModel { Cash = 10, UserId = this.LoggedUser.Id });

            if (purchaseAddBalanceModel != null)
            {
                AddLineToConsole("Adding money... OK");

            }
            else
            {
                AddLineToConsole("Adding money... FAILED");
            }
            AddLineToConsole("Drawal money...");
            var purchaseDrawalModel = _purchaseDataService.DrawalCash(new PurchaseModel { Cash = 5, UserId = this.LoggedUser.Id });
            if (purchaseDrawalModel != null)
            {
                AddLineToConsole("Drawal money... OK");

            }
            else
            {
                AddLineToConsole("Drawal money... FAILED");
            }


        }
        #endregion

        #region Logout testing
        private async Task LogoutTest()
        {

            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING LOGOUT...");

            //Demo Login OK
            var loginResponse = await AuthService.Logout();
            if (loginResponse.IsSuccess)
            {
                AddLineToConsole("Logout OK");
            }
            else
            {
                AddLineToConsole("Logout FAILED");
            }

        }

        #endregion

        private void LocalizationTest()
        {
            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING Localization...");
            AddLineToConsole($"Tranlate Accept: {_localizationService.Accept}");
            AddLineToConsole($"Tranlate Error: {_localizationService.Error}");
        }

        private async Task CaseEventDayly()
        {
            AddLineToConsole("-------------------------------------------");
            AddLineToConsole("TESTING Specific case CaseEventDayly...");
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
            var endDate = DateTime.Now.AddDays(1);
            var eventParking = new EventModel
            {
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                EndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 00),
                ParkingId = postedParking.Id,
                RepetitionEndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 00),
                RepetitionType = RepetitionType.Dayly
            };

            // postedParking.Status = Enums.Enums.ParkingStatus.Disabled;
            AddLineToConsole("----------Creating dayly events----------");
            AddLineToConsole($"->Start date:{eventParking.StartDate:G}");
            AddLineToConsole($"->End date:{eventParking.EndDate:G}");
            var result = await _eventDataService.CreateEventAsync(eventParking);
            if (result != null)
            {
                AddLineToConsole($"Created: {result.Count} dayly events for the created parking");
                AddLineToConsole("Creating dayly events OK");
            }
            else
            {
                AddLineToConsole("Creating dayly events FAILED");
            }
            AddLineToConsole("Getting posted parking's events ");
            var events = await _eventDataService.GetAllEventsAsync();
            AddLineToConsole($"Found: {events.Count} events.");
            if (events != null && events.Count > 0)
            {
                var count = 0;
                foreach (var item in events)
                {
                    count++;
                    AddLineToConsole($"---Event {count}:{Environment.NewLine} " +
                                     $"RepetitionId:{item.RepetitionId}{ Environment.NewLine}"+
                                     $"Start date:{item.StartDate:G}  End date:{item.EndDate:G}");
                }
            }

            AddLineToConsole("Deleting the parking");
            var deletedParking = await _parkingDataService.DeleteParkingsAsync(postedParking.Id);
            if (deletedParking != null)
            {
                AddLineToConsole("Deleting the parking OK");
            }
            else
            {
                AddLineToConsole("Deleting the parking FAILED");
            }
        }
    }
}