using Localization.Enums;
using Localization.Localize;
using System;
using System.Collections.Generic;
using Localization.TranslationResources;
using Xamarin.Forms;
using System.Globalization;

namespace Vaerator.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public class Item
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public override string ToString() => Text;
        }

        private List<Item> languages;
        public List<Item> Languages { get { return languages; } }

        private string languageSelectedText;
        public string LanguageSelectedText { get { return languageSelectedText + "\u25BC"; } }

        private int languageSelectedIndex = -1;
        public int LanguageSelectedIndex
        {
            get
            {
                return languageSelectedIndex;
            }
            set
            {
                if (languageSelectedIndex != value && value > -1)
                {
                    languageSelectedIndex = value;
                    string language = languages[value].Value;
                    if (Settings.Language != language)
                    {
                        Settings.Language = language;
                        if (Settings.Language == Settings.LanguageDefault)
                        {
                            ResourceContainer.Instance.RefreshCulture();
                        }
                        else
                        {
                            ResourceContainer.Instance.Culture = new CultureInfo(Settings.Language);
                        }
                    }
                    OnPropertyChanged(nameof(LanguageSelectedIndex));
                    RefreshLanguages();
                    languageSelectedText = languages[value].Text;
                    OnPropertyChanged(nameof(LanguageSelectedText));
                }
            }
        }

        public SettingsViewModel()
        {
            RefreshLanguages();
            LanguageSelectedIndex = languages.FindIndex(x => x.Value == Settings.Language);
        }

        public void RefreshLanguages()
        {
            CultureInfo culture = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            //string langNativeName = culture.NativeName.Split('(')[0].Trim();
            //string[] langCodeParts = culture.Name.Split('-');
            //string langCode = string.IsNullOrEmpty(langCodeParts[1]) ? langCodeParts[0] : langCodeParts[1];
            //string systemLang = string.Format(langNativeName + " ({0})", langCode);

            languages = new List<Item>(new[]
            {
                new Item { Text = string.Format(SettingsResources.SystemDefault, culture.TwoLetterISOLanguageName.ToUpper()), Value = Settings.LanguageDefault },
                new Item { Text = "English", Value = "EN" },
                new Item { Text = "French", Value = "FR" },
                new Item { Text = "Spanish", Value = "ES" },
            });

            OnPropertyChanged(nameof(Languages));
            OnPropertyChanged(nameof(LanguageSelectedIndex));
        }
    }
}
