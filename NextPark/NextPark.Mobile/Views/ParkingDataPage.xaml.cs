using System;
using System.Collections.Generic;
using NextPark.Mobile.Controls;
using NextPark.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace NextPark.Mobile.Views
{
    public partial class ParkingDataPage : ContentPage
    {
        public ParkingDataPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            if (BindingContext == null) return;
            if (BindingContext is BaseViewModel bvm)
            {
                bvm.InitializeAsync();
            }
        }

        void OnDateSelected(object sender, DateChangedEventArgs args)
        {
            int i = 0;
        }
    }
}
