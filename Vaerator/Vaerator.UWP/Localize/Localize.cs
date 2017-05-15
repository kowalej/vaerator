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
            return new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0].ToString());
        }
    }
}
