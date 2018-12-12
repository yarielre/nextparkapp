using NextPark.Mobile.ViewModels;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace NextPark.Mobile.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            if (BindingContext == null) return;
            if (BindingContext is BaseViewModel bvm) {
                bvm.InitializeAsync(MyMap);
            }
        }
    }
}
