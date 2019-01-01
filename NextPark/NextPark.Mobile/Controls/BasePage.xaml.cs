using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace NextPark.Mobile.Controls
{
    public partial class BasePage : ContentView
    {
        public static readonly BindableProperty HeaderBackTextProperty = BindableProperty.Create(nameof(HeaderBackText), typeof(string), typeof(BasePage), default(string), Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty HeaderBackActionProperty = BindableProperty.Create(nameof(HeaderBackAction), typeof(ICommand), typeof(BasePage), null, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty HeaderBackVisibleProperty = BindableProperty.Create(nameof(IsHeaderBackVisible), typeof(Boolean), typeof(BasePage), false, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty HeaderUserTextProperty = BindableProperty.Create(nameof(HeaderUserText), typeof(string), typeof(BasePage), default(string), Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty HeaderUserActionProperty = BindableProperty.Create(nameof(HeaderUserAction), typeof(ICommand), typeof(BasePage), null, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty HeaderMoneyProperty = BindableProperty.Create(nameof(HeaderMoney), typeof(string), typeof(BasePage), default(string), Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty HeaderMoneyActionProperty = BindableProperty.Create(nameof(HeaderMoneyAction), typeof(ICommand), typeof(BasePage), null, Xamarin.Forms.BindingMode.OneWay);


        public string HeaderBackText
        {
            get { return (string)GetValue(HeaderBackTextProperty); }
            set { SetValue(HeaderBackTextProperty, value); }
        }

        public ICommand HeaderBackAction
        {
            get { return (ICommand)this.GetValue(HeaderBackActionProperty); }
            set { SetValue(HeaderBackActionProperty, value); }
        }

        public Boolean IsHeaderBackVisible
        {
            get { return (bool)GetValue(HeaderBackVisibleProperty); }
            set { SetValue(HeaderBackVisibleProperty, value); }
        }

        public string HeaderUserText
        {
            get { return (string)GetValue(HeaderUserTextProperty); }
            set { SetValue(HeaderUserTextProperty, value); }
        }

        public ICommand HeaderUserAction
        {
            get { return (ICommand)this.GetValue(HeaderUserActionProperty); }
            set { SetValue(HeaderUserActionProperty, value); }
        }

        public string HeaderMoney
        {
            get { return (string)GetValue(HeaderMoneyProperty); }
            set { SetValue(HeaderMoneyProperty, value); }
        }

        public ICommand HeaderMoneyAction
        {
            get { return (ICommand)this.GetValue(HeaderMoneyActionProperty); }
            set { SetValue(HeaderMoneyActionProperty, value); }
        }

        public BasePage()
        {
            InitializeComponent();
        }
    }
}
