using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Vaerator.ViewModels
{
    public class Settings : BaseViewModel
    {
        static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        static Settings settings;
        public static Settings Current
        {
            get { return settings ?? (settings = new Settings()); }
        }

        const string BackgroundSimulationEnabledKey = "BackgroundSimulationEnabled";
        public bool BackgroundSimulationEnabled
        {
            get
            {
                return AppSettings.GetValueOrDefault<bool>(BackgroundSimulationEnabledKey, true);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue<bool>(BackgroundSimulationEnabledKey, value))
                    OnPropertyChanged();
            }
        }

        public const string LanguageDefault = "SYSTEM";
        const string LanguageKey = "Language";
        public string Language
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(LanguageKey, LanguageDefault);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue<string>(LanguageKey, value))
                    OnPropertyChanged();
            }
        }
    }
}
