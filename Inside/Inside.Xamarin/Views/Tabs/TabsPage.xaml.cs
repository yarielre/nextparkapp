using System;
using System.Diagnostics;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Services;
using Inside.Xamarin.ViewModels;
using Inside.Xamarin.Views.Rents;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Inside.Xamarin.Views.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabsPage : TabbedPage
    {
        private readonly TabsPageViewModel vm;

        public TabsPage()
        {
            InitializeComponent();

            //TODO: Removed if uneeded! 
            vm = MainViewModel.GetInstance().Tabs;
            BindingContext = vm;

            OnInit();
        }

        private void OnInit()
        {
            MessagesSubcriber();
            CurrentPageChanged += TabsPage_CurrentPageChanged;
        }

        private void TabsPage_CurrentPageChanged(object sender, System.EventArgs e)
        {
            var index = Children.IndexOf(CurrentPage);
            if (index == 2)
            {
                var aviability = AviabilityHelper.GetRentTimeLeft(RentsByHoursViewModel.GetIntance().ParkingRented);
                MessagingCenter.Send(aviability,Messages.TimeLeftRentByHours);
            }
            if (index == 3)
            {
                var aviability = AviabilityHelper.GetRentTimeLeft(RentsForMonthViewModel.GetIntance().ParkingRented);
                MessagingCenter.Send(aviability, Messages.TimeLeftForMonth);
            }
        }

        //private void OnCurrentPageChanged(object o, EventArgs eventArgs)
        //{
        //    CurrentPage= new RentsByHoursPage();
        //}

        private void MessagesSubcriber()
        {
            MessagingCenter.Subscribe<NavigationService>(this, Messages.FocusCoinsTab, sender => { FocusCoinsTab(); });
            MessagingCenter.Subscribe<string>(this, Messages.GoToRentByHours, go => { GoToRentByHours(); });
            MessagingCenter.Subscribe<string>(this, Messages.GoToRentForMonth, go => { GoToRentForMonth(); });
            MessagingCenter.Subscribe<string>(this, Messages.GoToHome, go => { GoToHome(); });
        }

        private void GoToRentForMonth()
        {
            CurrentPage = rentformonth;
        }

        private void FocusCoinsTab()
        {
            CurrentPage = coin;
        }

        private void GoToHome()
        {
            CurrentPage = home;
        }
        private void GoToRentByHours()
        {
            CurrentPage = rentbyhours;
        }
    }
}