using System;
using System.Collections.Generic;
using NextPark.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace NextPark.Mobile.Views
{
    public partial class AddParkingPage : ContentPage
    {
        public AddParkingPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
        }

        public void OnPriceMinChange(object sender, ValueChangedEventArgs args)
        {
            if (BindingContext is AddParkingViewModel) {
                AddParkingViewModel viewModel = BindingContext as AddParkingViewModel;
                viewModel.OnPriceMinChangedMethod(args.NewValue);
            }
        }

        public void OnPriceMaxChange(object sender, ValueChangedEventArgs args)
        {
            if (BindingContext is AddParkingViewModel) {
                AddParkingViewModel viewModel = BindingContext as AddParkingViewModel;
                viewModel.OnPriceMaxChangedMethod(args.NewValue);
            }
        }
    }
}
