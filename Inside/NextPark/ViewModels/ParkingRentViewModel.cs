using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Services;
using NextPark.Enums;
using NextPark.Models;
using Xamarin.Forms;

namespace Inside.Xamarin.ViewModels
{
    public class ParkingRentViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private string _aviability;
        private TimeSpan _rentFrom;
        private TimeSpan _rentTo;
        private bool _isRunning;
        private bool _isEnabled;
        private ImageSource _parkingPhoto;
        private double _parkingPrice;
       


        public ParkingRentViewModel(ParkingModel parking, bool isByHours)
        {
            OnItit(parking,isByHours);
        }

        public string RentTitle { get; set; }
        public bool IsByHours { get; set; }
        public bool IsByMonth { get; set; }
        public ParkingModel Parking { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsRunning
        {
            get => _isRunning;
            set => SetValue(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }

        public string IconNameBasedOnCategory { get; set; }

        public string Aviability
        {
            get => _aviability;
            set => SetValue(ref _aviability, value);
        }
        public ImageSource ParkingPhoto
        {
            get => _parkingPhoto;
            set => SetValue(ref _parkingPhoto, value);
        }
        public TimeSpan RentFrom
        {
            get => _rentFrom;
            set => SetValue(ref _rentFrom, value);
        }

        public TimeSpan RentTo
        {
            get => _rentTo;
            set => SetValue(ref _rentTo, value);
        }

        public bool IsEnable => IsAvialable();

        private bool IsAvialable()
        {
            return Aviability != "Not Avialable";
        }

        public double ParkingPrice
        {
            get => _parkingPrice;
            set => SetValue(ref _parkingPrice, value);
        }

        public ICommand RentCommand => new RelayCommand(Rent);

        private void OnItit(ParkingModel parking, bool isByHours)
        {
            SetImageSource(parking.ImageUrl);
            if (isByHours)
            {
                IsByHours = true;
                RentTitle = "Rent By Hours";
            }
            else
            {
                IsByMonth = true;
                RentTitle = "Rent For Months";
            }

            Parking = parking;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            RentFrom = DateTime.Now.TimeOfDay;
            RentTo = DateTime.Now.TimeOfDay;

            IconNameBasedOnCategory = parking.ParkingCategory.Category == "Business"
                ? "ic_location_green"
                : "ic_location_black";

            if (AviabilityHelper.IsAvialable(parking))
                Aviability = AviabilityHelper.GetAviability(parking);
            else
                Aviability = "Not Avialable";

            GetParkingPrice();
        }

        private void SetImageSource(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                ParkingPhoto = "add_photo";
                return;
            }
            ParkingPhoto = new UriImageSource
            {
                Uri = new Uri(string.Format("{0}{1}", HostSetting.BaseUrl, imageUrl))
            };
        }

        public double CalculateOrderPrice()
        {
            if (IsByHours)
            {
                var substract = RentTo - RentFrom;
                var hours = substract.Hours;
                var minutes = substract.Minutes;


                if (minutes > 0 && minutes <= 30)
                    minutes = 30;
                if (minutes > 30)
                {
                    hours++;
                    minutes = 0;
                }
                var hoursPrice = hours * Parking.ParkingCategory.HourPrice;
                var minutesPrice = minutes * (Parking.ParkingCategory.HourPrice / 2);
                var orderPrice = hoursPrice + minutesPrice;
                return orderPrice;
            }
            else
            {
                var substract = EndDate.Subtract(StartDate);
                var monthPrice = substract.Days*Parking.ParkingCategory.MonthPrice;
                return monthPrice;
            }
            
        }

        public async void Rent()
        {
            if (IsByHours)
            {
                if (RentFrom >= RentTo)
                {
                    DialogService.GetInstance().ShowInfoAlertOnMaster(
                        "Warning",
                        "The order start time must be shorter than order end time.");
                    return;
                }
                if (RentTo > Parking.ParkingEvent.EndTime)
                {
                    DialogService.GetInstance().ShowInfoAlertOnMaster(
                        "Warning",
                        $"The end time for the rent must be \n" +
                        $"shorter than {Parking.ParkingEvent.EndTime:t}");
                    return;
                }
            }
            if (IsByMonth)
            {
                if (StartDate >= EndDate)
                {
                    DialogService.GetInstance().ShowInfoAlertOnMaster(
                        "Warning",
                        "The order start date must be shorter than order end date.");
                    return;
                }
            }

            var orderPrice = CalculateOrderPrice();
                if (orderPrice > MainViewModel.GetInstance().CurrentUser.Coins)
                {
                    DialogService.GetInstance().ShowInfoAlertOnMaster(
                        "Warning",
                        "Sorry, You don't have enough money.\n" +
                        " Change the order time or buy some coins.");
                    return;
                }
            bool dialogResult;

            if (IsByHours)
            {
                 dialogResult = await DialogService.GetInstance().ShowDialogAlertOnMaster("Rent",
                    $"Do you want to make a rent \n" +
                    $"for {string.Format("{0:t}",RentFrom)} to for {string.Format("{0:t}", RentTo)} "+ 
                    $"with a price of {orderPrice} coins?");
            }
            else
            {
                dialogResult = await DialogService.GetInstance().ShowDialogAlertOnMaster("Rent",
                    $"Do you want to make a rent\n" +
                    $"from {StartDate:d} to {EndDate:d} \n" +
                    $"with a price of {orderPrice} coins?");
            }
                
                if (!dialogResult)
                    return;
                var order = new OrderModel
                {
                    StartDate = StartDate,
                    EndDate = EndDate,
                    StartTime = RentFrom,
                    EndTime = RentTo,
                    UserId = MainViewModel.GetInstance().CurrentUser.Id,
                    ParkingId = Parking.Id,
                    OrderStatus = OrderStatus.Actived,
                    Price = orderPrice
                };

            IsRunning = true;

            var response =
                    await InsideApi.Post<OrderModel, OrderModel>(HostSetting.OrderEndPoint + "/add", order);

            if (response.IsSuccess)
            {
                if (IsByHours)
                {
                    Parking.RentByHour = (OrderModel)response.Result;
                }
                else
                {
                    Parking.RentForMonth = (OrderModel)response.Result;
                }
               
                Parking.IsRented = true;
                var parkingEditedResponse = await DataService.GetInstance().EditParking(Parking);
                if (parkingEditedResponse.IsSuccess)
                {

                    
                    var updateUserCoinModel = new UpdateUserCoinModel
                    {
                        UserId = MainViewModel.GetInstance().CurrentUser.Id,
                        Coins = orderPrice
                    };
                    var updateUserResponse = await InsideApi.UpdateUserCoins(updateUserCoinModel);
                    if (!updateUserResponse.IsSuccess)
                    {
                        //TODO Hacer un rollback de la orden
                        DialogService.GetInstance().ShowInfoAlertOnMaster(
                            "Warning",
                            "Sorry, You don't have enough money.\n" +
                            " Change the order time or buy some coins.");
                        return;
                    }
                    MainViewModel.GetInstance().Menu.Coins = ((UserModel)updateUserResponse.Result).Coins.ToString();
                    IsRunning = false;

                    //Send Parking Model to the Home Page to update Pin icon
                    MessagingCenter.Send(Parking, Messages.ChangeIconAfterRent);
                    if (IsByHours)
                    {
                        RentsByHoursViewModel.GetIntance().ParkingRented = Parking;
                        RentsByHoursViewModel.GetIntance().TimeLeft = AviabilityHelper.GetRentTimeLeft(Parking);
                        RentsByHoursViewModel.GetIntance().HalfPrice = Parking.ParkingCategory.HourPrice * 0.6;
                        RentsByHoursViewModel.GetIntance().AnyParkingRentendByHours = true;
                        await NavigationService.GetInstance().BackOnMaster();
                        //Aqui se invoca un callback que se encuentra en la Tabspage con el fin de navegar al tab RentByHours.
                        MessagingCenter.Send("goToRentByHours", Messages.GoToRentByHours);
                    }
                    else
                    {
                        RentsForMonthViewModel.GetIntance().ParkingRented = Parking;
                        RentsForMonthViewModel.GetIntance().TimeLeft = AviabilityHelper.GetRentTimeLeft(Parking);
                        RentsForMonthViewModel.GetIntance().MonthPrice = Parking.ParkingCategory.MonthPrice * 30;
                        RentsForMonthViewModel.GetIntance().AnyParkigRentendForMonth = true;
                        await NavigationService.GetInstance().BackOnMaster();
                        //Aqui se invoca un callback que se encuentra en la Tabspage con el fin de navegar al tab RentByHours.
                        MessagingCenter.Send("goToRentForMonth", Messages.GoToRentForMonth);
                    }

                }
                else
                {
                    
                    //Borrando la orden de renta
                    if (IsByHours)
                    {
                       var deletedOrderResponse =
                            await InsideApi.DeleteOne<OrderModel>($"{HostSetting.OrderEndPoint}/deleteone",
                                Parking.RentByHour.Id);
                        if (deletedOrderResponse.IsSuccess)
                        {
                            Parking.RentByHour = null;
                        }
                    }
                    else
                    {
                       var deletedOrderResponse =
                            await InsideApi.DeleteOne<OrderModel>($"{HostSetting.OrderEndPoint}/deleteone",
                                Parking.RentForMonth.Id);
                        if (deletedOrderResponse.IsSuccess)
                        {
                            Parking.RentForMonth = null;
                        }
                    }
                       
                    
                }
            }

                else
                {
                    IsRunning = false;
                    await Application.Current.MainPage.DisplayAlert(
                        Languages.GeneralError,
                        Languages.ParkingInfoRentAlert,
                        Languages.GeneralAccept);
            }


        }
        private void GetParkingPrice()
        {
            if (Parking.ParkingCategory != null && Parking.ParkingType != null)
            {
                //Revisar los tipos de parqueos en la BD.
                ParkingPrice = Parking.ParkingType.Type == "For Month" ? Parking.ParkingCategory.MonthPrice : Parking.ParkingCategory.HourPrice;
            }

        }
    }
}