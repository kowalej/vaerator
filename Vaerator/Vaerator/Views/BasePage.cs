using Localization.Localize;
using System.Globalization;
using Vaerator.ViewModels;
using Xamarin.Forms;

namespace Vaerator.Views
{
    public class BasePage : ContentPage
    {
        protected override void OnAppearing()
        {
            string languageCode = Settings.Current.Language;
            if (languageCode == Settings.LanguageDefault)
                ResourceContainer.Instance.RefreshCulture(); 
            else ResourceContainer.Instance.Culture = new CultureInfo(languageCode); 
            base.OnAppearing();
        }
    }
}
