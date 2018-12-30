using System;
using System.Collections.Generic;
using NextPark.Mobile.ViewModels;

using Xamarin.Forms;

namespace NextPark.Mobile.Views
{
    public partial class BookingMapPage : ContentPage
    {
        public BookingMapPage()
        {
            InitializeComponent();
            if (BindingContext == null) return;
            if (BindingContext is BaseViewModel bvm)
            {
                BookingMapParams parameters = new BookingMapParams { Map = MyMap, Booking = null };
                bvm.InitializeAsync(parameters);
            }
        }
    }
}
