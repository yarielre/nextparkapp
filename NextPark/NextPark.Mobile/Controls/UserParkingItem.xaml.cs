using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace NextPark.Mobile.Controls
{
    public partial class UserParkingItem : ContentView
    {
        public static readonly BindableProperty TapActionProperty = BindableProperty.Create(nameof(TapAction), typeof(ICommand), typeof(UserParkingItem), null, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty IdentifierProperty = BindableProperty.Create(nameof(Identifier), typeof(int), typeof(UserParkingItem), default(int), Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(UserParkingItem), Color.Gray, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty PictureProperty = BindableProperty.Create(nameof(Picture), typeof(string), typeof(UserParkingItem), default(string), Xamarin.Forms.BindingMode.OneWay);

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

        public string Picture
        {
            get { return (string)this.GetValue(PictureProperty); }
            set { SetValue(PictureProperty, value); }
        }

        public UserParkingItem()
        {
            InitializeComponent();
        }
    }
}
