using Localization.Localize;
using System.Globalization;
using Xamarin.Forms;

[assembly: Dependency(typeof(Vaerator.UWP.Localize.Localize))]
namespace Vaerator.UWP.Localize
{
    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            return CultureInfo.CurrentUICulture;
        }

        public void SetLocale(CultureInfo culture)
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}
