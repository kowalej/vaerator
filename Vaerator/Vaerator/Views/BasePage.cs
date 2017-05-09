using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Vaerator.ViewModels;
using Localization.Localize;
using System.Globalization;

namespace Vaerator.Views
{
    public class BasePage : ContentPage
    {
        protected override void OnAppearing()
        {
            string languageCode = Settings.Current.Language;
            if (languageCode == Settings.LanguageDefault)
                ResourceContainer.Instance.RefreshCulture();
            else ResourceContainer.Instance.Culture = CultureInfo.CreateSpecificCulture(languageCode);
            base.OnAppearing();
        }
    }
}
