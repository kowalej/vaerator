using Foundation;
using Localization.Localize;
using System.Globalization;
using System.Threading;
using Xamarin.Forms;

[assembly: Dependency(typeof(Vaerator.iOS.Localize.Localize))]
namespace Vaerator.iOS.Localize
{
    public class Localize : ILocalize
    {
        public void SetLocale(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        public CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en";
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                netLanguage = iOSToDotnetLanguage(pref);
            }
            // This gets called a lot - try/catch can be expensive so consider caching or something.
            System.Globalization.CultureInfo culture = null;
            try
            {
                culture = new System.Globalization.CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException ex1)
            {
                // iOS locale not valid .NET culture (eg. "en-ES" : English in Spain).
                // Fallback to first characters, in this case "en".
                try
                {
                    var fallback = ToDotnetFallbackLanguage(new PlatformCulture(netLanguage));
                    culture = new System.Globalization.CultureInfo(fallback);
                }
                catch (CultureNotFoundException ex2)
                {
                    // iOS language not valid .NET culture, falling back to English.
                    culture = new System.Globalization.CultureInfo("en");
                }
            }
            return culture;
        }

        string iOSToDotnetLanguage(string iOSLanguage)
        {
            string netLanguage = iOSLanguage;
            // Certain languages need to be converted to CultureInfo equivalent.
            switch (iOSLanguage)
            {
                case "ms-MY":   // "Malaysian (Malaysia)" not supported .NET culture.
                case "ms-SG":   // "Malaysian (Singapore)" not supported .NET culture.
                    netLanguage = "ms"; // Closest supported.
                    break;
                case "gsw-CH":  // "Schwiizertüütsch (Swiss German)" not supported .NET culture.
                    netLanguage = "de-CH"; // Closest supported.
                    break;
                    // Add more application-specific cases here (if required).
                    // ONLY use cultures that have been tested and known to work
            }
            return netLanguage;
        }

        string ToDotnetFallbackLanguage(PlatformCulture platformCulture)
        {
            var netLanguage = platformCulture.LanguageCode; // Use the first part of the identifier (two chars, usually).
            switch (platformCulture.LanguageCode)
            {
                case "pt":
                    netLanguage = "pt-PT"; // Fallback to Portuguese (Portugal).
                    break;
                case "gsw":
                    netLanguage = "de-CH"; // Equivalent to German (Switzerland) for this app.
                    break;
                    // Add more application-specific cases here (if required).
                    // ONLY use cultures that have been tested and known to work
            }
            return netLanguage;
        }
    }
}
