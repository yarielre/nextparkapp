using System;
using System.Threading.Tasks;
using NextPark.Mobile.Services.Data;
using NextPark.Mobile.Settings;
using NextPark.Mobile.UIModels;
using NextPark.Mobile.ViewModels;
using NextPark.Models;
using Xamarin.Forms;

namespace NextPark.Mobile.Services
{
    public class PushService : IPushService
    {
        private readonly INavigationService _navService;
        private readonly IDialogService _dialogService;
        private readonly IAuthService _authService;
        private readonly IProfileService _profileService;
        private readonly IOrderDataService _orderDataService;
        private readonly IParkingDataService _parkingDataService;

        public PushService(INavigationService navigationService,
                           IDialogService dialogService,
                           IAuthService authService,
                           IProfileService profileService,
                           IParkingDataService parkingDataService,
                           IOrderDataService orderDataService)
        {
            _dialogService = dialogService;
            _navService = navigationService;
            _authService = authService;
            _profileService = profileService;
            _parkingDataService = parkingDataService;
            _orderDataService = orderDataService;
        }

        public void Start()
        {
            PushSettings.UpdateDeviceId();
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(1), () => { PushServiceTask(); return false; });
        }

        /*
         * CODE to go directly on order page if push of an expired order arrives when app were closed
         * 
        public async Task<bool> NotificationHandler()
        {
            if (PushSettings.NewNotification && PushSettings.ExpiredOrder) {
                PushSettings.ExpiredOrder = false;

                // Search order
                await _navService.NavigateToAsync<MoneyViewModel>();

                // Return that the page is changed
                return true;
            }

            // Nothing to do
            return false;
        }
        */

        private async void PushServiceTask()
        {
            try
            {
                if (PushSettings.NewNotification)
                {
                    PushSettings.NewNotification = false;

                    // Refresh user data, balance could be updated
                    await _profileService.RefreshUserData();

                    if (PushSettings.ExpiredOrder)
                    {
                        // Search Order
                        UIBookingModel booking = null;

                        var order = _profileService.GetUserOrderById(PushSettings.ExpiredOrderId);
                        if (order == null) {
                            order = await _orderDataService.GetOrderAsync(PushSettings.ExpiredOrderId);
                        }

                        if (order != null) {

                            if ((_authService.IsUserAuthenticated()) && (AuthSettings.User.Id == order.UserId) && (order.OrderStatus != Enums.OrderStatus.Finished))
                            {
                                var parking = (ParkingModel)_profileService.GetParkingById(order.ParkingId);
                                if (parking == null)
                                {
                                    parking = await _parkingDataService.GetParkingAsync(order.ParkingId);
                                }
                                if (parking != null)
                                {
                                    // Create UIBookingModel
                                    booking = new UIBookingModel(order);
                                    booking.UID = order.Id;
                                    booking.Address = parking.Address;
                                    booking.Cap = parking.Cap.ToString();
                                    booking.City = parking.City;
                                    booking.Parking = (ParkingModel)parking;
                                }
                            }
                        }
                        if (booking != null)
                        {
                            // Order found
                            var result = await _dialogService.ShowConfirmAlert(PushSettings.NotificationArgs.Title, PushSettings.NotificationArgs.Message, "Vai alla prenotazione", "Chiudi");
                            if (result)
                            {
                                await _navService.NavigateToAsync<BookingMapViewModel>(booking);
                            }
                        } else {
                            await _dialogService.ShowAlert(PushSettings.NotificationArgs.Title, PushSettings.NotificationArgs.Message);
                        }
                    }
                    else
                    {
                        await _dialogService.ShowAlert(PushSettings.NotificationArgs.Title, PushSettings.NotificationArgs.Message);
                    }
                }
            } finally {
                Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(1), () => { PushServiceTask(); return false; });
            }
        }

    }
}
