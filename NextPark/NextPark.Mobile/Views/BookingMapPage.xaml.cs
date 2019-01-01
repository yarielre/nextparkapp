using System;
using System.Collections.Generic;
using NextPark.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace NextPark.Mobile.Views
{
    public partial class BookingMapPage : ContentPage
    {
        public BookingMapPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            if (BindingContext == null) return;
            if (BindingContext is BaseViewModel bvm)
            {
                BookingMapParams parameters = new BookingMapParams { Map = MyMap, Booking = null };
                bvm.InitializeAsync(parameters);
            }
        }
    }
}
