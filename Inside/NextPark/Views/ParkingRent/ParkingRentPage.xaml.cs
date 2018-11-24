using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Inside.Xamarin.Views.ParkingRent
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ParkingRentPage : ContentPage
    {
        public ParkingRentPage()
        {
            InitializeComponent();
        }

        //void MainPageSizeChanged(object sender, EventArgs e)
        //{
        //    if (App.Current.MainPage.Width > App.Current.MainPage.Height)
        //        mainContent.Orientation = StackOrientation.Horizontal;
        //    else
        //        mainContent.Orientation = StackOrientation.Vertical;
        //}
    }
}