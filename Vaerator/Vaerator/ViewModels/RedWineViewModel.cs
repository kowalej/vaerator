using Localization.TranslationResources;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Vaerator.ViewModels
{
	public class RedWineViewModel : BeverageBaseViewModel
	{
        public RedWineViewModel()
		{
            DURATION_MIN = 15;
            DURATION_MAX = 45;
            DURATION_STEP_SIZE = 5;
            RECOMMENDED_DURATION = 25;
        }

        public override void InitializeDefaults()
        {
            durationValue = Settings.RedWineDurationPref;
            base.InitializeDefaults();
        }

        public override void SavePrefs()
        {
            Settings.RedWineDurationPref = durationValue;
        }
	}
}
