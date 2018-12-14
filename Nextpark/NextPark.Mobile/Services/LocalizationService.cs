using NextPark.Mobile.Resources;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace NextPark.Mobile.Services
{
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }

    public class PlatformCulture
    {
        public PlatformCulture(string platformCultureString)
        {
            if (string.IsNullOrEmpty(platformCultureString))
            {
                throw new ArgumentException(@"Expected culture identifier", nameof(platformCultureString));
            }

            PlatformString = platformCultureString.Replace("_", "-");
            var dashIndex = PlatformString.IndexOf("-", StringComparison.Ordinal);
            if (dashIndex > 0)
            {
                var parts = PlatformString.Split('-');
                LanguageCode = parts[0];
                LocaleCode = parts[1];
            }
            else
            {
                LanguageCode = PlatformString;
                LocaleCode = "";
            }
        }

        public string PlatformString { get; private set; }
        public string LanguageCode { get; private set; }
        public string LocaleCode { get; private set; }

        public override string ToString()
        {
            return PlatformString;
        }

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
            public string Accept { get { return Localize.Accept_Global ?? NotAvailable; } }
            public string Cancel { get { return Localize.Cancel_Global ?? NotAvailable; } }
            public string Error { get { return Localize.Error_Global ?? NotAvailable; } }

        }
    }
}
