using NextPark.Mobile.Services;
using NextPark.Mobile.ViewModels;
using System;

namespace NextPark.Mobile.Infrastructure
{
    public class LocatorViewModel
    {
        public StartUpViewModel StartUp => IoCSettings.Resolve<StartUpViewModel>();
        public AddParkingViewModel AddParking { get { return (AddParkingViewModel)IoCSettings.Resolve<AddParkingViewModel>(); } }
        public BookingViewModel BookNow { get { return (BookingViewModel)IoCSettings.Resolve<BookingViewModel>(); } }
        public MoneyViewModel Budget { get { return (MoneyViewModel)IoCSettings.Resolve<MoneyViewModel>(); } }
        public ParkingDataViewModel ParkingData { get { return (ParkingDataViewModel)IoCSettings.Resolve<ParkingDataViewModel>(); } }
        public RegisterViewModel Register { get { return (RegisterViewModel)IoCSettings.Resolve<RegisterViewModel>(); } }
        public UserBookingViewModel UserBooking { get { return (UserBookingViewModel)IoCSettings.Resolve<UserBookingViewModel>(); } }
        public UserDataViewModel UserData { get { return (UserDataViewModel)IoCSettings.Resolve<UserDataViewModel>(); } }
        public UserParkingViewModel UserParking { get { return (UserParkingViewModel)IoCSettings.Resolve<UserParkingViewModel>(); } }
        public UserProfileViewModel Profile { get { return (UserProfileViewModel)IoCSettings.Resolve<UserProfileViewModel>(); } }

        public HomeViewModel Home
        {
            get
            {
                HomeViewModel vm = null;

                try
                {
                    vm = IoCSettings.Resolve<HomeViewModel>();
                }
                catch (Exception e)
                {

                }
                return vm;
            }
        }
    }
}
