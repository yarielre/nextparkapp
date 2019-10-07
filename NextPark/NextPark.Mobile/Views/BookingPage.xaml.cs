using System;
using System.ComponentModel;
using System.Collections.Generic;
using NextPark.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace NextPark.Mobile.Views
{
    public partial class BookingPage : ContentPage
    {
        public static readonly BindableProperty TimeProperty = BindableProperty.Create(nameof(Time), typeof(string), typeof(BookingPage), default(string), Xamarin.Forms.BindingMode.OneWay);

        private static bool pictureExpanded = false;

        public TimeSpan Time
        {
            get { return (TimeSpan)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public BookingPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            if (BindingContext == null) return;
        }

        private void ExpandParkingPicture()
        {
            if (!pictureExpanded)
            {
                var yDiff = App.Current.MainPage.Height - 144 - StackPicture.Height;
                StackPicture.LayoutTo(new Rectangle(0, 0, StackPicture.Width, yDiff + StackPicture.Height), 400, Easing.SpringOut);
                StackInfo.TranslateTo(0, yDiff, 400, Easing.SpringOut);
                parkingPicture.Aspect = Aspect.AspectFit;
                pictureExpanded = true;
            }
        }

        private void ShrinkParkingPicture()
        {
            if (pictureExpanded)
            {
                StackPicture.LayoutTo(new Rectangle(0, 0, StackPicture.Width, 150), 400, Easing.SpringOut);
                StackInfo.TranslateTo(0, 0, 400, Easing.SpringOut);
                parkingPicture.Aspect = Aspect.AspectFill;
                pictureExpanded = false;
            }
        }

        public void OnPictureSwipeDown(object sender, EventArgs e)
        {
            if (!pictureExpanded)
            {
                ExpandParkingPicture();
            }
        }

        public void OnPictureSwipeUp(object sender, EventArgs e)
        {            
            if (pictureExpanded) { 
                ShrinkParkingPicture();
            }
        }

        public void OnPictureTapped(object sender, EventArgs e)
        {
            if (pictureExpanded)
            {
                ShrinkParkingPicture();
            } else
            {
                ExpandParkingPicture();
            }
        }
    }
}
