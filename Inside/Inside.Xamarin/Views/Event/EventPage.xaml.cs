using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inside.Domain.Enums;
using Inside.Xamarin.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Inside.Xamarin.Views.Event
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EventPage : ContentPage
	{
		public EventPage ()
		{
			InitializeComponent ();
		}
	    private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
	    {
	        if (e.SelectedItem is SelectableItem<MyDayOfWeek> item)
	        {
	            // toggle the selection property
	            item.IsSelected = !item.IsSelected;
	            MessagingCenter.Send(item, Messages.DayRepeatChange);
	        }

	        // deselect the item
	        ((ListView)sender).SelectedItem = null;
	    }
    }
}