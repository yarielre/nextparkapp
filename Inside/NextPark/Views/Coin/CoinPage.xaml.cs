using Inside.Xamarin.Helpers;
using Inside.Xamarin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Inside.Xamarin.Views.Coin
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CoinPage : ContentPage
	{
		public CoinPage ()
		{
			InitializeComponent ();

            MessagingCenter.Subscribe<CoinPageViewModel, bool>(this, Messages.Purchase, (sender, arg) => {
                // On Purchase do:
                if (!arg) return;
                this.DisplayAlert("In-App Billing Service", "Purchase successfull", "Close");
            });
        }
	}
}