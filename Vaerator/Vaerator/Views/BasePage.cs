﻿using Localization.Localize;
using System.Globalization;
using Vaerator.ViewModels;
using Xamarin.Forms;

namespace Vaerator.Views
{
    public abstract class BasePage : ContentPage
    {
        protected override void OnAppearing()
        {
            RefreshCulture();
            SetTranslationText();
            base.OnAppearing();
        }

        protected abstract void SetTranslationText();

        protected void RefreshCulture()
        {
            string languageCode = Settings.Current.Language;
            if (languageCode == Settings.LanguageDefault)
                ResourceContainer.Instance.RefreshCulture();
            else ResourceContainer.Instance.Culture = new CultureInfo(languageCode);
        }
    }
}
