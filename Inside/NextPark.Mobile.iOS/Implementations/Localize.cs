using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Foundation;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Interfaces;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Inside.Xamarin.iOS.Implementations.Localize))]
namespace Inside.Xamarin.iOS.Implementations
{
    class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en";
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                netLanguage = iOSToDotnetLanguage(pref);
            }
            CultureInfo ci = null;
            try
            {
                ci = new CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException e1)
            {
                //IOS locale not valid .Net culture (eg. "en-ES" : English in Spain)
                //fallback to first characters, in this case "en"
                try
                {
                    var fallback = ToDotnetFallbackLanguage(new PlatformCulture(netLanguage));
                    ci = new CultureInfo(fallback);
                }
                catch (CultureNotFoundException e2)
                {
                    ci = new CultureInfo("en");
                }
            }
            return ci;
        }

        private string iOSToDotnetLanguage(string IosLanguage)
        {
            var netLanguage = IosLanguage;
            switch (IosLanguage)
            {
                case "ms-MY": //Malaysian (Malaysia)" not support .NET culture
                case "ms-SG": //Malaysian (Singapore)" not support .NET culture
                    netLanguage = "ms"; //closest supported
                    break;               
                case "gsw-CH": //"Schwiizertuutsch (Swiss German)" not supported .NET
                    netLanguage = "de-CH"; //closest supported
                    break;
            }
            return netLanguage;
        }

        public void SetLocale(CultureInfo ci)
        {
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        } 

        string ToDotnetFallbackLanguage(PlatformCulture platCulture)
        {
            var netLanguage = platCulture.LanguageCode; // use the first part of the identifier (two chars, usually)
            switch (platCulture.LanguageCode)
            {
                case "pt":
                    netLanguage = "pt-PT"; // fallback to Portuguese (Portugal)
                    break;
                case "gsw":
                    netLanguage = "de-CH"; //equivalent to german (Switzerland) for this app
                    break;
            }
            return netLanguage;
        }
    }
}