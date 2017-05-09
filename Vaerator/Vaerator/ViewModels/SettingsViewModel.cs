using Localization.Enums;
using Localization.Localize;
using System;
using System.Collections.Generic;
using Localization.TranslationResources;
using Xamarin.Forms;

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

        private List<Item> languages { get; }
        public List<Item> Languages { get { return languages; } }

        private int languageSelectedIndex;
        public int LanguageSelectedIndex
        {
            get
            {
                return languageSelectedIndex;
            }
            set
            {
                if (languageSelectedIndex != value)
                {
                    languageSelectedIndex = value;
                    string language = languages[value].Value;
                    Settings.Language = language;
                    OnPropertyChanged();
                }
            }
        }

        public SettingsViewModel()
        {
            languages = new List<Item>(new[]
            {
                new Item { Text = string.Format(SettingsResources.SystemDefault, DependencyService.Get<ILocalize>().GetCurrentCultureInfo().DisplayName), Value=Settings.LanguageDefault },
                new Item { Text = "English", Value = "EN" },
                new Item { Text = "French", Value = "FR" },
                new Item { Text = "Spanish", Value = "ES" },
            });

            LanguageSelectedIndex = languages.FindIndex(x => x.Value == Settings.Language);
        }
    }
}
