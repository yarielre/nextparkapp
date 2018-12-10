using System.Globalization;
using NextPark.Mobile.Core.Resources;
using Xamarin.Forms;

namespace NextPark.Mobile.Core.Services
{
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }

    public class LocalizationService
    {
        public LocalizationService()
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Localize.Culture = ci;
            DependencyService.Get<ILocalize>().SetLocale(ci);
        }

    }
}
