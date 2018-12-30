using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Mobile.ViewModels;

using Xamarin.Forms;

namespace NextPark.Mobile.Views
{
    public partial class UserBookingPage : ContentPage
    {
        public UserBookingPage()
        {
            InitializeComponent();
            if (BindingContext == null) return;
            if (BindingContext is BaseViewModel bvm)
            {
                bvm.InitializeAsync();
            }
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // don't do anything if we just de-selected the row.
            if (e.SelectedItem == null) return;

            // Deselect the item.
            if (sender is ListView bookingList)
            {
                bookingList.SelectedItem = null;
            }
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            // don't do anything if we just de-selected the row.
            if (e.Item == null) return;

            // Deselect the item.
            if (sender is ListView bookingList)
            {
                bookingList.SelectedItem = null;
            }
        }
    }
}
