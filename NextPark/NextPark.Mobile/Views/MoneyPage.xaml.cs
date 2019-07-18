using System;
using System.Collections.Generic;
using NextPark.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace NextPark.Mobile.Views
{
    public partial class MoneyPage : ContentPage
    {
        public MoneyPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            if (BindingContext == null) return;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<MoneyPage>(this, "RefreshData", (sender) => {
                var result = ((BaseViewModel)BindingContext).RefreshDataAsync();
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<MoneyPage>(this, "RefreshData");
        }
    }
}
