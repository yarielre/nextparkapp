using NextPark.Mobile.Resources;
using System;
using System.Globalization;
using NextPark.Mobile.Interfaces;
using Xamarin.Forms;

namespace NextPark.Mobile.Services
{
   
    public class LocalizationService : ILocalizationService
    {
        public string NotAvailable = "NotAvailable";

        public LocalizationService()
        {
            try
            {
                var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                Localize.Culture = ci;
                DependencyService.Get<ILocalize>().SetLocale(ci);
            }
            catch
            {

            }
        }
        public string Accept => Localize.Accept_Global ?? NotAvailable;
        public string Cancel => Localize.Cancel_Global ?? NotAvailable;
        public string Error => Localize.Error_Global ?? NotAvailable;
    }
}
