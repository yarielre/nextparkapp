using System;
using NextPark.Mobile.ViewModels;

using Xamarin.Forms;

namespace NextPark.Mobile.Views
{
    public partial class UserProfilePage : ContentPage
    {
        public UserProfilePage()
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
