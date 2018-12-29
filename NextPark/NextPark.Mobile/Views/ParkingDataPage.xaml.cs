using System;
using System.Collections.Generic;
using NextPark.Mobile.ViewModels;

using Xamarin.Forms;

namespace NextPark.Mobile.Views
{
    public partial class ParkingDataPage : ContentPage
    {
        public ParkingDataPage()
        {
            InitializeComponent();
            if (BindingContext == null) return;
            if (BindingContext is BaseViewModel bvm)
            {
                bvm.InitializeAsync();
            }
        }
    }
}
