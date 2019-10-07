using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace NextPark.Mobile.Views
{
    public partial class ReservationPage : ContentPage
    {
        private static bool pictureExpanded = false;

        public ReservationPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            if (BindingContext == null) return;
        }

        private void ExpandParkingPicture()
        {
            if (!pictureExpanded)
            {
                var yDiff = Xamarin.Forms.Application.Current.MainPage.Height - 144 - StackPicture.Height;
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
                StackPicture.LayoutTo(new Rectangle(0, 0, StackPicture.Width, 170), 400, Easing.SpringOut);
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
            if (pictureExpanded)
            {
                ShrinkParkingPicture();
            }
        }

        public void OnPictureTapped(object sender, EventArgs e)
        {
            if (pictureExpanded)
            {
                ShrinkParkingPicture();
            }
            else
            {
                ExpandParkingPicture();
            }
        }
    }
}
