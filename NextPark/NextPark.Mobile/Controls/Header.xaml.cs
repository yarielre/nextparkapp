using System;
using System.Windows.Input;
using Xamarin.Forms;
using NextPark.Mobile.Controls;

namespace NextPark.Mobile.Controls
{
    public partial class Header : TemplatedView
    {
        // PROPERTIES
        //-----------

        // Back
        public static readonly BindableProperty BackTextProperty = BindableProperty.Create(nameof(BackText), typeof(string), typeof(Header), default(string), Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty BackActionProperty = BindableProperty.Create(nameof(BackAction), typeof(ICommand), typeof(Header), null, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty BackVisibleProperty = BindableProperty.Create(nameof(BackVisible), typeof(Boolean), typeof(Header), false, Xamarin.Forms.BindingMode.OneWay);
        // User
        public static readonly BindableProperty UserTextProperty = BindableProperty.Create(nameof(UserText), typeof(string), typeof(Header), "Login", Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty UserActionProperty = BindableProperty.Create(nameof(UserAction), typeof(ICommand), typeof(Header), null, Xamarin.Forms.BindingMode.OneWay);
        // Money
        public static readonly BindableProperty MoneyProperty = BindableProperty.Create(nameof(Money), typeof(string), typeof(Header), "0", Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty MoneyActionProperty = BindableProperty.Create(nameof(MoneyAction), typeof(ICommand), typeof(Header), null, Xamarin.Forms.BindingMode.OneWay);

        public string BackText
        {
            get { return (string)GetValue(BackTextProperty); }
            set { SetValue(BackTextProperty, value); }
        }

        public ICommand BackAction
        {
            get { return (ICommand)this.GetValue(BackActionProperty); }
            set { SetValue(BackActionProperty, value); }
        }

        public Boolean BackVisible
        {
            get { return (bool)GetValue(BackVisibleProperty); }
            set { SetValue(BackVisibleProperty, value); }
        }

        public string UserText
        {
            get { return (string)GetValue(UserTextProperty); }
            set { SetValue(UserTextProperty, value); }
        }

        public ICommand UserAction
        {
            get { return (ICommand)this.GetValue(UserActionProperty); }
            set { SetValue(UserActionProperty, value); }
        }

        public string Money
        {
            get { return (string)GetValue(MoneyProperty); }
            set { SetValue(MoneyProperty, value); }
        }

        public ICommand MoneyAction
        {
            get { return (ICommand)this.GetValue(MoneyActionProperty); }
            set { SetValue(MoneyActionProperty, value); }
        }

        public Header()
        {
            InitializeComponent();
        }
    }
}