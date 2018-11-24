using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Services;
using NextPark.Models;
using Plugin.ExternalMaps;
using Xamarin.Forms;

namespace Inside.Xamarin.ViewModels
{
    public class RentsByHoursViewModel : BaseViewModel
    {
        private static RentsByHoursViewModel _instance;
        private bool _anyParkigRentendByHours;

        private double _halfPrice;

        private ParkingModel _parkingRented;
        private string _timeLeft;

        public RentsByHoursViewModel()
        {
            _instance = this;
            MessagingCenter.Subscribe<string>(this, Messages.TimeLeftRentByHours,
                aviability => { TimeLeft = aviability; });
            OnInit();
        }

        public ParkingModel ParkingRented
        {
            get => _parkingRented;
            set => SetValue(ref _parkingRented, value);
        }

        public double HalfPrice
        {
            get => _halfPrice;
            set => SetValue(ref _halfPrice, value);
        }

        public string TimeLeft
        {
            get => _timeLeft;
            set => SetValue(ref _timeLeft, value);
        }

        public bool AnyParkingRentendByHours
        {
            get => _anyParkigRentendByHours;
            set => SetValue(ref _anyParkigRentendByHours, value);
        }

        public ICommand TerminateRentCommand => new RelayCommand(TerminateRent);
        public ICommand RenovateMinutesCommand => new RelayCommand(RenovateMinutes);
        public ICommand NavigateToParkingCommand =>new RelayCommand(NavigateToParking);

        private async void NavigateToParking()
        {
           await CrossExternalMaps.Current.NavigateTo("Some Message", Double.Parse(ParkingRented.Latitude), Double.Parse(ParkingRented.Longitude));
        }


        public ICommand RenovateHourCommand => new RelayCommand(RenovateHour);


        private void OnInit()
        {
            GetParkinRentedByHours();
        }

        private async void TerminateRent()
        {
            var response = await InsideApi.DeleteOne<ParkingModel>(HostSetting.OrderEndPoint + "/terminateorder",
                ParkingRented.RentByHour.Id);
            if (response.IsSuccess && response.Result != null)
                try
                {
                    var parkingEdited = response.Result as ParkingModel;
                    RefreshView();
                    MessagingCenter.Send(parkingEdited, Messages.ParkingEdited);
                    MessagingCenter.Send(parkingEdited, Messages.ChangeIconAfterTerminateRent);
                    MessagingCenter.Send("goToHome",Messages.GoToHome);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
        }

        private async void RenovateHour()
        {
            //Falta saber como se prefiere si se desabilita el boton de renovar en caso que al sumar los 
            //30 minutos sobrepace el tiempo disponible del parqueo
            // o si se mantiene habilitado el boton y se chequea y si sobrepaza el tiempo se muestra un alert al usuario.
            var user = new UserModel
            {
                Coins = MainViewModel.GetInstance().CurrentUser.Coins,
                Id = MainViewModel.GetInstance().CurrentUser.Id
            };

            if (user.Coins < ParkingRented.ParkingCategory.HourPrice)
            {
                DialogService.GetInstance().ShowInfoAlertOnMaster(
                    "Warning",
                    "Sorry, You don't have enough money.\n" +
                    "Buy some coins.");
                return;
            }
            ParkingRented.RentByHour.EndTime = ParkingRented.RentByHour.EndTime.Add(new TimeSpan(0, 1, 0, 0));
            user.Coins = user.Coins - ParkingRented.ParkingCategory.HourPrice;
            var renovateOrder = new RenovateOrder
            {
                Order = ParkingRented.RentByHour,
                User = user
            };
            var response =
                await InsideApi.Post<RenovateOrder, OrderModel>(HostSetting.OrderEndPoint + "/renovateorder",
                    renovateOrder);
            if (!response.IsSuccess)
            {
                DialogService.GetInstance().ShowInfoAlertOnMaster(
                    "Error",
                    "Error trying to renovate the rent");
                ParkingRented.RentByHour.EndTime = ParkingRented.RentByHour.EndTime.Subtract(new TimeSpan(0, 1, 0, 0));
                return;
            }
            ParkingRented.RentByHour = response.Result as OrderModel;
            MainViewModel.GetInstance().CurrentUser.Coins = user.Coins;
            MainViewModel.GetInstance().Menu.Coins = user.Coins.ToString();
            TimeLeft = AviabilityHelper.GetRentTimeLeft(ParkingRented);
        }

        private async void RenovateMinutes()
        {
            //Falta saber como se prefiere si se desabilita el boton de renovar en caso que al sumar los 
            //30 minutos sobrepace el tiempo disponible del parqueo
            // o si se mantiene habilitado el boton y se chequea y si sobrepaza el tiempo se muestra un alert al usuario.
            var user = new UserModel
            {
                Coins = MainViewModel.GetInstance().CurrentUser.Coins,
                Id = MainViewModel.GetInstance().CurrentUser.Id
            };

            if (user.Coins < HalfPrice)
            {
                DialogService.GetInstance().ShowInfoAlertOnMaster(
                    "Warning",
                    "Sorry, You don't have enough money.\n" +
                    "Buy some coins.");
                return;
            }
            ParkingRented.RentByHour.EndTime = ParkingRented.RentByHour.EndTime.Add(new TimeSpan(0, 0, 30, 0));
            user.Coins = user.Coins - HalfPrice;
            var renovateOrder = new RenovateOrder
            {
                Order = ParkingRented.RentByHour,
                User = user
            };
            var response =
                await InsideApi.Post<RenovateOrder, OrderModel>(HostSetting.OrderEndPoint + "/renovateorder",
                    renovateOrder);
            if (!response.IsSuccess)
            {
                DialogService.GetInstance().ShowInfoAlertOnMaster(
                    "Error",
                    "Error trying to renovate the rent");
                ParkingRented.RentByHour.EndTime = ParkingRented.RentByHour.EndTime.Subtract(new TimeSpan(0, 0, 30, 0));
                return;
            }
            ParkingRented.RentByHour = response.Result as OrderModel;
            MainViewModel.GetInstance().CurrentUser.Coins = user.Coins;
            MainViewModel.GetInstance().Menu.Coins = user.Coins.ToString();
            TimeLeft = AviabilityHelper.GetRentTimeLeft(ParkingRented);
        }

        private async void GetParkinRentedByHours()
        {
            var parking = await DataService.GetInstance()
                .GetParkingRentedByHours(MainViewModel.GetInstance().CurrentUser.Id);
            ParkingRented = parking.Result as ParkingModel;
            AnyParkingRentendByHours = ParkingRented != null;
            GetHalfPrice();
            if (ParkingRented != null)
                TimeLeft = AviabilityHelper.GetRentTimeLeft(ParkingRented);
        }

        private void GetHalfPrice()
        {
            if (ParkingRented != null)
                HalfPrice = ParkingRented.ParkingCategory.HourPrice * 0.6;
        }

        private void RefreshView()
        {
            ParkingRented = null;
            TimeLeft = "";
            HalfPrice = 0;
            AnyParkingRentendByHours = false;
        }

        public static RentsByHoursViewModel GetIntance()
        {
            return _instance ?? new RentsByHoursViewModel();
        }
    }
}