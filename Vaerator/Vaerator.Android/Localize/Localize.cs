using Localization.Localize;
using System.Globalization;
using System.Threading;
using Xamarin.Forms;

[assembly: Dependency(typeof(Vaerator.Droid.Localize.Localize))]
namespace Vaerator.Droid.Localize
{
    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en";
            var androidLocale = Java.Util.Locale.Default;
            netLanguage = AndroidToDotnetLanguage(androidLocale.ToString().Replace("_", "-"));

            // This gets called a lot - try/catch can be expensive so consider caching or something.
            System.Globalization.CultureInfo culture = null;
            try
            {
                culture = new System.Globalization.CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException)
            {
                // Android locale not valid .NET culture (eg. "en-ES" : English in Spain).
                // Fallback to first characters, in this case "en".
                try
                {
                    var fallback = ToDotnetFallbackLanguage(new PlatformCulture(netLanguage));
                    culture = new System.Globalization.CultureInfo(fallback);
                }
                catch (CultureNotFoundException)
                {
                    // Android language not valid .NET culture, falling back to English.
                    culture = new System.Globalization.CultureInfo("en");
                }
            }
            return culture;
        }

        string AndroidToDotnetLanguage(string androidLocale)
        {
            var netLanguage = androidLocale;
            // Certain languages need to be converted to CultureInfo equivalent.
            switch (androidLocale)
            {
                case "ms-BN":   // "Malaysian (Brunei)" not supported .NET culture.
                case "ms-MY":   // "Malaysian (Malaysia)" not supported .NET culture.
                case "ms-SG":   // "Malaysian (Singapore)" not supported .NET culture.
                    netLanguage = "ms"; // Closest supported.
                    break;
                case "in-ID":  // "Indonesian (Indonesia)" has different code in  .NET.
                    netLanguage = "id-ID"; // Correct code for .NET.
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
                case "gsw":
                    netLanguage = "de-CH"; // equivalent to German (Switzerland) for this app.
                    break;
                    // Add more application-specific cases here (if required).
                    // ONLY use cultures that have been tested and known to work
            }
            return netLanguage;
        }
    }
}