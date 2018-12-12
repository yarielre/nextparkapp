using NextPark.Mobile.Resources;
using System.Globalization;
using Xamarin.Forms;

namespace NextPark.Mobile.Services
{
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }

    public class LocalizationService : ILocalizationService
    {
        public string NotAvailable = "NotAvailable";

        public LocalizationService()
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Localize.Culture = ci;
            DependencyService.Get<ILocalize>().SetLocale(ci);
        }
        public string Accept { get { return Localize.Accept_Global ?? NotAvailable; } }
        public string Cancel { get { return Localize.Cancel_Global ?? NotAvailable; } }
        public string Error { get { return Localize.Error_Global ?? NotAvailable; } }

    }
}
