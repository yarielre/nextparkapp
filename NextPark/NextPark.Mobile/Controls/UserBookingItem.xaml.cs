using System;
using System.Collections.Generic;
using System.Windows.Input;

using Xamarin.Forms;

namespace NextPark.Mobile.Controls
{
    public partial class UserBookingItem : ContentView
    {
        public static readonly BindableProperty TapActionProperty = BindableProperty.Create(nameof(TapAction), typeof(ICommand), typeof(UserBookingItem), null, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(UserBookingItem), Color.Gray, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty DelActionProperty = BindableProperty.Create(nameof(DelAction), typeof(ICommand), typeof(UserBookingItem), null, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty DelVisibleProperty = BindableProperty.Create(nameof(DelVisible), typeof(Boolean), typeof(UserBookingItem), false, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty IdentifierProperty = BindableProperty.Create(nameof(Identifier), typeof(int), typeof(UserBookingItem), default(int), Xamarin.Forms.BindingMode.OneWay);

        public static readonly BindableProperty AddressProperty = BindableProperty.Create(nameof(Address), typeof(string), typeof(UserBookingItem), default(string), Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty CapProperty = BindableProperty.Create(nameof(Cap), typeof(string), typeof(UserBookingItem), default(string), Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty CityProperty = BindableProperty.Create(nameof(City), typeof(string), typeof(UserBookingItem), default(string), Xamarin.Forms.BindingMode.OneWay);

        public static readonly BindableProperty TimeProperty = BindableProperty.Create(nameof(Time), typeof(string), typeof(UserBookingItem), default(string), Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty TimeColorProperty = BindableProperty.Create(nameof(TimeColor), typeof(Color), typeof(UserBookingItem), Color.Black, Xamarin.Forms.BindingMode.OneWay);
        //public static readonly BindableProperty TimeVisibleProperty = BindableProperty.Create(nameof(TimeVisible), typeof(bool), typeof(UserBookingItem), false, Xamarin.Forms.BindingMode.OneWay);
        //public static readonly BindableProperty StartDateTimeProperty = BindableProperty.Create(nameof(StartDateTime), typeof(string), typeof(UserBookingItem), default(string), Xamarin.Forms.BindingMode.OneWay);
        //public static readonly BindableProperty EndDateTimeProperty = BindableProperty.Create(nameof(EndDateTime), typeof(string), typeof(UserBookingItem), default(string), Xamarin.Forms.BindingMode.OneWay);
        //public static readonly BindableProperty DateTimeVisibleProperty = BindableProperty.Create(nameof(DateTimeVisible), typeof(bool), typeof(UserBookingItem), false, Xamarin.Forms.BindingMode.OneWay);


        public static readonly BindableProperty SwipeActionProperty = BindableProperty.Create(nameof(SwipeAction), typeof(ICommand), typeof(UserBookingItem), null, Xamarin.Forms.BindingMode.OneWay);


        public ICommand TapAction
        {
            get { return (ICommand)this.GetValue(TapActionProperty); }
            set { SetValue(TapActionProperty, value); }
        }

        public int Identifier
        {
            get { return (int)this.GetValue(IdentifierProperty); }
            set { SetValue(IdentifierProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color)this.GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public ICommand DelAction
        {
            get { return (ICommand)this.GetValue(DelActionProperty); }
            set { SetValue(DelActionProperty, value); }
        }

        public Boolean DelVisible
        {
            get { return (bool)this.GetValue(DelVisibleProperty); }
            set { SetValue(DelVisibleProperty, value); }
        }

        public string Address
        {
            get { return (string)this.GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        public string Cap
        {
            get { return (string)this.GetValue(CapProperty); }
            set { SetValue(CapProperty, value); }
        }

        public string City
        {
            get { return (string)this.GetValue(CityProperty); }
            set { SetValue(CityProperty, value); }
        }

        public string Time
        {
            get { return (string)this.GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public Color TimeColor
        {
            get { return (Color)this.GetValue(TimeColorProperty); }
            set { SetValue(TimeColorProperty, value); }
        }

        public ICommand SwipeAction
        {
            get { return (ICommand)this.GetValue(SwipeActionProperty); }
            set { SetValue(SwipeActionProperty, value); }
        }

        public UserBookingItem()
        {
            InitializeComponent();
            this.SetValue(DelVisibleProperty, false);
            OnPropertyChanged();
        }

        void OnSwiped(object sender, SwipedEventArgs e)
        {
            if (e.Direction == SwipeDirection.Left)
            {
                SetValue(DelVisibleProperty, true);
            }
            else
            {
                SetValue(DelVisibleProperty, false);
            }
            OnPropertyChanged();
        }
    }
}
