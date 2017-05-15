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

        public const int BeverageDurationNoSetting = -1;
        const string RedWineDurationPrefKey = "RedWineDurationPref";
        public int RedWineDurationPref
        {
            get
            {
                return AppSettings.GetValueOrDefault<int>(RedWineDurationPrefKey, BeverageDurationNoSetting);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue<int>(RedWineDurationPrefKey, value))
                    OnPropertyChanged();
            }
        }

        const string WhiteWineDurationPrefKey = "WhiteWineDurationPref";
        public int WhiteWineDurationPref
        {
            get
            {
                return AppSettings.GetValueOrDefault<int>(WhiteWineDurationPrefKey, BeverageDurationNoSetting);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue<int>(WhiteWineDurationPrefKey, value))
                    OnPropertyChanged();
            }
        }

        const string WhiskeySpiritsDurationPrefKey = "WhiskeySpiritsDurationPref";
        public int WhiskeySpiritsDurationPref
        {
            get
            {
                return AppSettings.GetValueOrDefault<int>(WhiskeySpiritsDurationPrefKey, BeverageDurationNoSetting);
            }
            set
            {
                if (AppSettings.AddOrUpdateValue<int>(WhiskeySpiritsDurationPrefKey, value))
                    OnPropertyChanged();
            }
        }
    }
}
