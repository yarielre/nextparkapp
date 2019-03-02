﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using NextPark.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

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

        public BookingPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            if (BindingContext == null) return;
        }
    }
}
