using System;
using System.Collections.Generic;
using NextPark.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace NextPark.Mobile.Views
{
    public partial class AddEventPage : ContentPage
    {
        public AddEventPage()
        {
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            InitializeComponent();

        }

        public void OnWeekDayTapped(object sender, ItemTappedEventArgs e)
        {
            AddEventViewModel bvm = BindingContext as AddEventViewModel;
            bvm.OnWeekDaySelected(e.Item);
        }
    }
}
