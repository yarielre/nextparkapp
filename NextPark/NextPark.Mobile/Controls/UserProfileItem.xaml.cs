using System;
using System.Windows.Input;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NextPark.Mobile.Controls
{
    public partial class UserProfileItem : ContentView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(UserProfileItem), default(string), Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty ClickActionProperty = BindableProperty.Create(nameof(ClickAction), typeof(ICommand), typeof(UserProfileItem), null, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(UserProfileItem), Color.Gray, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(UserProfileItem), default(string), Xamarin.Forms.BindingMode.OneWay);

        public ICommand ClickAction
        {
            get { return (ICommand)this.GetValue(ClickActionProperty); }
            set { SetValue(ClickActionProperty, value); }
        }

        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color)this.GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public string Icon
        {
            get { return (string)this.GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public UserProfileItem()
        {
            InitializeComponent();
        }
    }
}
