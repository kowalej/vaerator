using Localization.TranslationResources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vaerator.ViewModels
{
    public abstract class BaseBeverageViewModel : BaseViewModel
    {
        protected int DURATION_MIN = 15;
        protected int DURATION_MAX = 45;
        protected int DURATION_STEP_SIZE = 5;
        protected int RECOMMENDED_DURATION = 25;

        public int DurationMinimum { get { return DURATION_MIN; } }
        public int DurationMaximum { get { return DURATION_MAX; } }

        public string DurationMinimumText { get { return string.Format(BeverageResources.DurationFormat, DURATION_MIN); } }
        public string DurationMaximumText { get { return string.Format(BeverageResources.DurationFormat, DURATION_MAX); } }

        protected int durationValue;

        public string DurationValueText
        {
            get
            {
                string recommended = durationValue == RECOMMENDED_DURATION ? BeverageResources.Recommended : string.Empty;
                return string.Format(BeverageResources.DurationText, durationValue, recommended);
            }
        }

        public int DurationValue
        {
            get { return durationValue; }
            set
            {
                // Find nearest step.
                int nearestStep = (int)Math.Round(value / (float)DURATION_STEP_SIZE) * DURATION_STEP_SIZE;

                // If we are changing and within 50% of step size we change.
                if (nearestStep != durationValue && (Math.Abs(nearestStep - value)) / DURATION_STEP_SIZE < 0.5f)
                {
                    durationValue = nearestStep;
                    OnPropertyChanged(nameof(DurationValue));
                    OnPropertyChanged(nameof(DurationValueText));
                }
                else
                {
                    // Run little task to update value once and awhile.
                    if (!updateSliderValue)
                    {
                        updateSliderValue = true;
                        Task.Run(() => UpdateSliderValue());
                    }
                }
            }
        }

        protected int timeRemaining = 0;
        public int TimeRemaining { get { return timeRemaining; } }

        bool updateSliderValue = false;
        async void UpdateSliderValue()
        {
            if (updateSliderValue)
            {
                await Task.Delay(100);
                OnPropertyChanged(nameof(DurationValue));
                OnPropertyChanged(nameof(DurationValueText));
                updateSliderValue = false;
            }
        }

        // Set inital value, needs to be called when page loads.
        public virtual void InitializeDefaults()
        {
            if (durationValue == Settings.BeverageDurationNoSetting || durationValue < DurationMinimum || durationValue > DurationMaximum)
                durationValue = RECOMMENDED_DURATION;

            OnPropertyChanged(nameof(DurationValue));
            OnPropertyChanged(nameof(DurationValueText));
        }

        // Save preferences.
        public abstract void SavePrefs();
    }
}
