using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace NextPark.Mobile.Controls
{
    public partial class SelButton : ContentView
    {
        public static readonly BindableProperty InfoProperty = BindableProperty.Create(nameof(Info), typeof(string), typeof(SelButton), default(string), Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty SubInfoProperty = BindableProperty.Create(nameof(SubInfo), typeof(string), typeof(SelButton), default(string), Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty TapActionProperty = BindableProperty.Create(nameof(TapAction), typeof(ICommand), typeof(SelButton), null, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty BtnBorderColorProperty = BindableProperty.Create(nameof(BtnBorderColor), typeof(Color), typeof(SelButton), Color.Gray, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty BtnBackgroundColorProperty = BindableProperty.Create(nameof(BtnBackgroundColor), typeof(Color), typeof(SelButton), Color.White, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty BtnTextColorProperty = BindableProperty.Create(nameof(BtnTextColor), typeof(Color), typeof(SelButton), Color.Gray, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty IdentifierProperty = BindableProperty.Create(nameof(ID), typeof(string), typeof(SelButton), null, Xamarin.Forms.BindingMode.OneWay);
        public static readonly BindableProperty SelectedProperty = BindableProperty.Create(nameof(Selected), typeof(Boolean), typeof(SelButton), false, Xamarin.Forms.BindingMode.TwoWay, propertyChanged: OnStatusChanged);
        public static readonly BindableProperty EnabledProperty = BindableProperty.Create(nameof(Enabled), typeof(Boolean), typeof(SelButton), false, Xamarin.Forms.BindingMode.TwoWay, propertyChanged: OnStatusChanged);

        //private readonly _isSelected = false;
        private string _identifier = "0";

        private static void OnSelectionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SelButton)bindable;

            if (newValue is bool)
            {
                if ((bool)newValue == true)
                {
                    control.BtnBackgroundColor = (Color)Application.Current.Resources["NextParkColor1"];
                    control.BtnBorderColor = (Color)Application.Current.Resources["NextParkColor1"];
                    control.BtnTextColor = Color.White;
                    //control._isSelected = true;
                }
                else if (control.Enabled)
                {
                    control.BtnBackgroundColor = Color.White;
                    control.BtnBorderColor = (Color)Application.Current.Resources["NextParkColor1"];
                    control.BtnTextColor = (Color)Application.Current.Resources["NextParkColor1"];
                } else
                {
                    control.BtnBackgroundColor = Color.White;
                    control.BtnBorderColor = Color.Gray;
                    control.BtnTextColor = Color.Gray;
                    //control._isSelected = false;
                }
            }
        }

        private static void OnEnableChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SelButton)bindable;

            if (newValue is bool)
            {
                if ((bool)newValue == false)
                {
                    control.BtnBackgroundColor = Color.White;
                    control.BtnBorderColor = Color.Gray;
                    control.BtnTextColor = Color.Gray;
                }
                else
                {
                    control.BtnBackgroundColor = Color.White;
                    control.BtnBorderColor = (Color)Application.Current.Resources["NextParkColor1"];
                    control.BtnTextColor = (Color)Application.Current.Resources["NextParkColor1"];
                }
            }
        }

        private static void OnStatusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SelButton)bindable;

            if (control.Enabled)
            {
                if (control.Selected)
                {
                    control.BtnBackgroundColor = (Color)Application.Current.Resources["NextParkColor1"];
                    control.BtnBorderColor = (Color)Application.Current.Resources["NextParkColor1"];
                    control.BtnTextColor = Color.White;
                } else {
                    control.BtnBackgroundColor = Color.White;
                    control.BtnBorderColor = (Color)Application.Current.Resources["NextParkColor1"];
                    control.BtnTextColor = (Color)Application.Current.Resources["NextParkColor1"];
                }
            } else {
                control.BtnBackgroundColor = Color.White;
                control.BtnBorderColor = Color.Gray;
                control.BtnTextColor = Color.Gray;
            }
        }

        public string Info
        {
            get { return (string)GetValue(InfoProperty); }
            set { SetValue(InfoProperty, value); }
        }

        public string SubInfo
        {
            get { return (string)GetValue(SubInfoProperty); }
            set { SetValue(SubInfoProperty, value); }
        }

        public ICommand TapAction
        {
            get { return (ICommand)this.GetValue(TapActionProperty); }
            set { SetValue(TapActionProperty, value); }
        }

        public Color BtnBorderColor
        {
            get { return (Color)this.GetValue(BtnBorderColorProperty); }
            set { SetValue(BtnBorderColorProperty, value); }
        }

        public Color BtnBackgroundColor
        {
            get { return (Color)this.GetValue(BtnBackgroundColorProperty); }
            set { SetValue(BtnBackgroundColorProperty, value); }
        }

        public Color BtnTextColor
        {
            get { return (Color)this.GetValue(BtnTextColorProperty); }
            set { SetValue(BtnTextColorProperty, value); }
        }

        public SelButton()
        {
            InitializeComponent();
        }

        public bool Selected
        {
            get { return (bool)this.GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        public bool Enabled
        {
            get { return (bool)this.GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }

        public string ID
        {
            get { return this._identifier; }
            set
            {
                SetValue(IdentifierProperty, value);
                this._identifier = (string)value;
            }
        }
    }
}
