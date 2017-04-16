using System.Globalization;

namespace Localization.Localize
{
    public interface ILocalize {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo culture);
    }
}

