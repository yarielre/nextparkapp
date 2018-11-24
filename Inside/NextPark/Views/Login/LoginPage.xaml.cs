using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Inside.Xamarin.Views.Login
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage ()
		{
			InitializeComponent ();
		    
		}

        void MainPageSizeChanged(object sender, EventArgs e)
        {
            //if (App.Current.MainPage.Width > App.Current.MainPage.Height)
            //{
            //    mainContent.Orientation = StackOrientation.Horizontal;
            //    mainContent.Margin = new Thickness(0, 2, 40, 0);
            //    mainContent.Padding = new Thickness(0, 0, 0, 0);
            //    //toggleContent.Margin = new Thickness(0);
            //    entryPasswordContent.Margin = new Thickness(0);
            //    formContent.Margin = new Thickness(0);
            //    UsernameFormContent.Margin = new Thickness(0, 0, 0, 0);
            //    buttonContent.Margin = new Thickness(0,0,0,40);
            //    buttonContent.VerticalOptions = new LayoutOptions(LayoutAlignment.Start, false);
            //    activityContent.WidthRequest = 30;
            //    activityContent.HeightRequest = 30;
            //}
            //else
            //{
            //    mainContent.Orientation = StackOrientation.Vertical;
            //    mainContent.Padding = new Thickness(10, 20, 10, 5);
            //    mainContent.Margin = new Thickness(0, 0, 0, 0);
            //    //toggleContent.Margin = new Thickness(0,10,0,0);
            //    entryPasswordContent.Margin = new Thickness(0,10,0,0);

            //    formContent.Margin = new Thickness(0, 20, 10, 0);
            //    UsernameFormContent.Margin = new Thickness(0, 20, 0, 0);

            //    buttonContent.Margin = new Thickness(0, 0, 0, 10);
            //    buttonContent.VerticalOptions = new LayoutOptions(LayoutAlignment.End, true);
            //    activityContent.WidthRequest = 60;
            //    activityContent.HeightRequest = 60;
            //}
        }
    }
}