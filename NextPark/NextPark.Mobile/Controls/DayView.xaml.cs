using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NextPark.Mobile.Controls
{

    public partial class DayView : ContentView
    {
        private ScrollView myScrollingView;

        public void ScrollTo(int position)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(500), () => { ScrollDayView(position); return false; });
        }

        private async void ScrollDayView(int position)
        {
            await myScrollingView.ScrollToAsync(0, position, true);
        }

        public DayView()
        {
            InitializeComponent();
        }

        void Handle_ChildAdded(object sender, Xamarin.Forms.ElementEventArgs e)
        {
            if (sender is ScrollView) {
                myScrollingView = (ScrollView)sender;
            }
        }
    }
}
