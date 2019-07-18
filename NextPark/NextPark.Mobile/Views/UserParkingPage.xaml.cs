using System;
using System.Collections.Generic;
using NextPark.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace NextPark.Mobile.Views
{
    public partial class UserParkingPage : ContentPage
    {
        public UserParkingPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            if (BindingContext == null) return;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ParkingListView.SelectedItem = null;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        { 
            ParkingListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<UserParkingPage>(this, "RefreshData", (sender) => {
                var result = ((BaseViewModel)BindingContext).RefreshDataAsync();
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<UserParkingPage>(this, "RefreshData");
        }
    }
}
