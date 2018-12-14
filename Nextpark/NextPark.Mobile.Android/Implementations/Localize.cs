using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using Java.Util;
using NextPark.Mobile.Droid.Implementations;
using NextPark.Mobile.Services;

[assembly: Dependency(typeof(Localize))]

namespace NextPark.Mobile.Droid.Implementations
{
    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en";
            var androidLocale = Locale.Default;
            netLanguage = AndroidToDotnetLanguage(androidLocale.ToString().Replace("_", "-"));
            CultureInfo ci = null;
            try
            {
                ci = new CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException el)
            {
                ci = new CultureInfo("en");
            }
            return ci;
        }

        public void SetLocale(CultureInfo ci)
        {
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        private string AndroidToDotnetLanguage(string androidLanguage)
        {
            var netLanguage = androidLanguage;
            //certain languages need to be converted to CultureInfo equivalent
            switch (androidLanguage)
            {
                case "ms-BN": //Malaysian (Brunei)" not support .NET culture
                case "ms-MY": //Malaysian (Malaysia)" not support .NET culture
                case "ms-SG": //Malaysian (Singapore)" not support .NET culture
                    netLanguage = "ms"; //closest supported
                    break;
                case "in-ID": //Indonesian (Indonesia)" has ifferent code in .NET
                    netLanguage = "id-ID"; //correct code for .NET
                    break;
                case "gsw-CH": //"Schwiizertuutsch (Swiss German)" not supported .NET
                    netLanguage = "de-CH"; //closest supported
                    break;
            }
            return netLanguage;
        }

        private string ToDotnetFallbackLanguage(PlatformCulture platCulture)
        {
            var netLanguage = platCulture.LanguageCode;
            switch (platCulture.LanguageCode)
            {
                case "gsw":
                    netLanguage = "de-CH"; //equivalent to german (Switzerland) for this app
                    break;
            }
            return netLanguage;
        }
    }
}