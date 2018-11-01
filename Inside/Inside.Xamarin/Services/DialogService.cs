using Inside.Xamarin.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Inside.Xamarin.Services
{
    public class DialogService
    {
        public async void ShowInfoAlertOnMaster(string title, string message)
        {
            await App.Navigator.CurrentPage.DisplayAlert(title, message, Languages.GeneralAccept);
        }
        public async Task<bool> ShowDialogAlertOnMaster(string title, string message)
        {
            return await App.Navigator.CurrentPage.DisplayAlert(title, message, Languages.GeneralAccept, Languages.GeneralCancel);
        }

        private static DialogService Instance
        {
            get;
            set;
        }
        public static DialogService GetInstance()
        {
            return Instance ?? new DialogService();
        }
    }
}
