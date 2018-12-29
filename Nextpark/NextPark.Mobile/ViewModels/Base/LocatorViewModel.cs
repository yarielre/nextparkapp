﻿using NextPark.Mobile.Services;
using NextPark.Mobile.ViewModels;
using System;

namespace NextPark.Mobile.Infrastructure
{
    public class LocatorViewModel
    {
        public StartUpViewModel StartUp => IoCSettings.Resolve<StartUpViewModel>();
        public AddParkingViewModel AddParking { get { return (AddParkingViewModel)IoCSettings.Resolve<AddParkingViewModel>(); } }
        public MoneyViewModel Budget { get { return (MoneyViewModel)IoCSettings.Resolve<MoneyViewModel>(); } }
        public RegisterViewModel Register { get { return (RegisterViewModel)IoCSettings.Resolve<RegisterViewModel>(); } }
        public UserParkingViewModel UserParking { get { return (UserParkingViewModel)IoCSettings.Resolve<UserParkingViewModel>(); } }
        public UserDataViewModel UserData { get { return (UserDataViewModel)IoCSettings.Resolve<UserDataViewModel>(); } }
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
