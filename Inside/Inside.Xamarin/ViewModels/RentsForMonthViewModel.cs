using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Inside.Domain.Models;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Services;
using Plugin.ExternalMaps;
using Xamarin.Forms;

namespace Inside.Xamarin.ViewModels
{
   public class RentsForMonthViewModel:BaseViewModel
    {
        private static RentsForMonthViewModel _instance;
        private bool _anyParkigRentendForMonth;

        private ParkingModel _parkingRented;
        private string _timeLeft;
        private double _monthPrice;

        public RentsForMonthViewModel()
        {
            _instance = this;
            MessagingCenter.Subscribe<string>(this, Messages.TimeLeftForMonth,
                aviability => { TimeLeft = aviability; });
            OnInit();
        }

        public ParkingModel ParkingRented
        {
            get => _parkingRented;
            set => SetValue(ref _parkingRented, value);
        }

        public double MonthPrice
        {
            get => _monthPrice;
            set => SetValue(ref _monthPrice, value);
        }

        public string TimeLeft
        {
            get => _timeLeft;
            set => SetValue(ref _timeLeft, value);
        }

        public bool AnyParkigRentendForMonth
        {
            get => _anyParkigRentendForMonth;
            set => SetValue(ref _anyParkigRentendForMonth, value);
        }

        public ICommand TerminateRentCommand => new RelayCommand(TerminateRent);
        public ICommand RenovateDayCommand => new RelayCommand(RenovateDay);
        public ICommand RenovateMonthCommand => new RelayCommand(RenovateMonth);
        public ICommand NavigateToParkingCommand => new RelayCommand(NavigateToParking);

        private async void NavigateToParking()
        {
            await CrossExternalMaps.Current.NavigateTo("Some Message", Double.Parse(ParkingRented.Latitude), Double.Parse(ParkingRented.Longitude));

        }


        private void OnInit()
        {
            GetParkinRentedForMonth();
        }

        private async void TerminateRent()
        {
            var response = await InsideApi.DeleteOne<ParkingModel>(HostSetting.OrderEndPoint + "/terminateorder",
                ParkingRented.RentForMonth.Id);
            if (response.IsSuccess && response.Result != null)
                try
                {
                    var parkingEdited = response.Result as ParkingModel;
                    RefreshView();
                    MessagingCenter.Send(parkingEdited, Messages.ParkingEdited);
                    MessagingCenter.Send(parkingEdited, Messages.ChangeIconAfterTerminateRent);
                    MessagingCenter.Send("goToHome", Messages.GoToHome);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
        }

        private async void RenovateDay()
        {
            //Falta saber como se prefiere si se desabilita el boton de renovar en caso que al sumar los 
            //30 minutos sobrepace el tiempo disponible del parqueo
            // o si se mantiene habilitado el boton y se chequea y si sobrepaza el tiempo se muestra un alert al usuario.
            var user = new UserModel
            {
                Coins = MainViewModel.GetInstance().CurrentUser.Coins,
                Id = MainViewModel.GetInstance().CurrentUser.Id
            };

            if (user.Coins < ParkingRented.ParkingCategory.MonthPrice)
            {
                DialogService.GetInstance().ShowInfoAlertOnMaster(
                    "Warning",
                    "Sorry, You don't have enough money.\n" +
                    "Buy some coins.");
                return;
            }
            ParkingRented.RentForMonth.EndDate = ParkingRented.RentForMonth.EndDate.AddDays(1);
            user.Coins = user.Coins - ParkingRented.ParkingCategory.MonthPrice;
            var renovateOrder = new RenovateOrder
            {
                Order = ParkingRented.RentForMonth,
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
                ParkingRented.RentForMonth.EndDate =
                    ParkingRented.RentForMonth.EndDate.Subtract(new TimeSpan(1, 0, 0, 0));
                return;
            }
            ParkingRented.RentForMonth = response.Result as OrderModel;
            MainViewModel.GetInstance().CurrentUser.Coins = user.Coins;
            MainViewModel.GetInstance().Menu.Coins = user.Coins.ToString();
            TimeLeft = AviabilityHelper.GetRentTimeLeft(ParkingRented);
        }
        private async void RenovateMonth()
        {
            //Falta saber como se prefiere si se desabilita el boton de renovar en caso que al sumar los 
            //30 minutos sobrepace el tiempo disponible del parqueo
            // o si se mantiene habilitado el boton y se chequea y si sobrepaza el tiempo se muestra un alert al usuario.
            var user = new UserModel
            {
                Coins = MainViewModel.GetInstance().CurrentUser.Coins,
                Id = MainViewModel.GetInstance().CurrentUser.Id
            };

            if (user.Coins < MonthPrice)
            {
                DialogService.GetInstance().ShowInfoAlertOnMaster(
                    "Warning",
                    "Sorry, You don't have enough money.\n" +
                    "Buy some coins.");
                return;
            }
            ParkingRented.RentForMonth.EndDate = ParkingRented.RentForMonth.EndDate.AddMonths(1);
            user.Coins = user.Coins - MonthPrice;
            var renovateOrder = new RenovateOrder
            {
                Order = ParkingRented.RentForMonth,
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
               
                //TODO Esto tengo que mirarlo mañana
                ParkingRented.RentByHour.EndTime = ParkingRented.RentByHour.EndTime.Subtract(ParkingRented.RentForMonth.EndDate.AddMonths(1).TimeOfDay);
                return;
            }
            ParkingRented.RentForMonth = response.Result as OrderModel;
            MainViewModel.GetInstance().CurrentUser.Coins = user.Coins;
            MainViewModel.GetInstance().Menu.Coins = user.Coins.ToString();
            TimeLeft = AviabilityHelper.GetRentTimeLeft(ParkingRented);
        }

        private async void GetParkinRentedForMonth()
        {
            var parking = await DataService.GetInstance()
                .GetParkingRentedForMonth(MainViewModel.GetInstance().CurrentUser.Id);
            ParkingRented = parking.Result as ParkingModel;
            AnyParkigRentendForMonth = ParkingRented != null;
            GetMonthPrice();
            if (ParkingRented != null)
                TimeLeft = AviabilityHelper.GetRentTimeLeft(ParkingRented);
        }

        private void GetMonthPrice()
        {
            if (ParkingRented != null)
                MonthPrice = ParkingRented.ParkingCategory.MonthPrice * 24;
        }

        private void RefreshView()
        {
            ParkingRented = null;
            TimeLeft = "";
            MonthPrice = 0;
            AnyParkigRentendForMonth = false;
        }

        public static RentsForMonthViewModel GetIntance()
        {
            return _instance ?? new RentsForMonthViewModel();
        }
    }
}
    

