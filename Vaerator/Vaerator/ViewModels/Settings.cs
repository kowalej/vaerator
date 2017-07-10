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
                return AppSettings.GetValueOrDefault(BackgroundSimulationEnabledKey, true);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue(BackgroundSimulationEnabledKey, value))
                    OnPropertyChanged();
            }
        }

        public const string LanguageDefault = "SYSTEM";
        const string LanguageKey = "Language";
        public string Language
        {
            get
            {
                return AppSettings.GetValueOrDefault(LanguageKey, LanguageDefault);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue(LanguageKey, value))
                    OnPropertyChanged();
            }
        }

        public const int BeverageDurationNoSetting = -1;
        const string RedWineDurationPrefKey = "RedWineDurationPref";
        public int RedWineDurationPref
        {
            get
            {
                return AppSettings.GetValueOrDefault(RedWineDurationPrefKey, BeverageDurationNoSetting);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue(RedWineDurationPrefKey, value))
                    OnPropertyChanged();
            }
        }

        const string WhiteWineDurationPrefKey = "WhiteWineDurationPref";
        public int WhiteWineDurationPref
        {
            get
            {
                return AppSettings.GetValueOrDefault(WhiteWineDurationPrefKey, BeverageDurationNoSetting);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue(WhiteWineDurationPrefKey, value))
                    OnPropertyChanged();
            }
        }

        const string WhiskeySpiritsDurationPrefKey = "WhiskeySpiritsDurationPref";
        public int WhiskeySpiritsDurationPref
        {
            get
            {
                return AppSettings.GetValueOrDefault(WhiskeySpiritsDurationPrefKey, BeverageDurationNoSetting);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue(WhiskeySpiritsDurationPrefKey, value))
                    OnPropertyChanged();
            }
        }
    }
}
