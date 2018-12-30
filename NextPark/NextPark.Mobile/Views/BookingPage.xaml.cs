using System;
using System.ComponentModel;
using System.Collections.Generic;
using NextPark.Mobile.ViewModels;
using Xamarin.Forms;

namespace NextPark.Mobile.Views
{
    public partial class BookingPage : ContentPage
    {
        public static readonly BindableProperty TimeProperty = BindableProperty.Create(nameof(Time), typeof(string), typeof(BookingPage), default(string), Xamarin.Forms.BindingMode.OneWay);

        public TimeSpan Time
        {
            get { return (TimeSpan)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        private static double selectedValue;

        public BookingPage()
        {
            InitializeComponent();
            if (BindingContext == null) return;
            if (BindingContext is BaseViewModel bvm)
            {
                bvm.InitializeAsync();
            }
        }

        /*
        void OnTimePickerPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Time")
            {
                selectedValue = Convert.ToDouble(_timePicker.Time.TotalHours);
                OnPropertyChanged();
            }
        }
        */
    }
}
