using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace NextPark.Mobile.Views
{
    public partial class UserParkingPage : ContentPage
    {
        public UserParkingPage()
        {
            InitializeComponent();
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ParkingListView.SelectedItem = null;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        { 
            ParkingListView.SelectedItem = null;
        }
    }
}
