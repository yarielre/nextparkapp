using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inside.Xamarin.ViewModels;
using Inside.Xamarin.Views.Master;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Inside.Xamarin.Views.Load
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoadPage : ContentPage
	{
		public LoadPage ()
		{
			InitializeComponent ();
            InitAsync();
		}

	   private async void  InitAsync()
	    {
	       await MainViewModel.GetInstance().RestoreLoginData();
           MainViewModel.GetInstance().RentsByHours = new RentsByHoursViewModel();
           MainViewModel.GetInstance().RentsForMonth = new RentsForMonthViewModel();
           Application.Current.MainPage = new MasterView();
	    }
	}
}