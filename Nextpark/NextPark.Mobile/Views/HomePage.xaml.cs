using NextPark.Mobile.ViewModels;
using System;
using System.Windows.Input;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NextPark.Mobile.CustomControls;

namespace NextPark.Mobile.Views
{
    public partial class HomePage : ContentPage
    {

        public HomePage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            if (BindingContext == null) return;
            if (BindingContext is HomeViewModel bvm)
            {
                bvm.MyMapContainer = MapContainer;
            }
        }
    }
}
