using Localization.TranslationResources;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Vaerator.ViewModels
{
	public class WhiteWineViewModel : BeverageBaseViewModel
	{
        public WhiteWineViewModel()
		{
            DURATION_MIN = 15;
            DURATION_MAX = 45;
            DURATION_STEP_SIZE = 5;
            RECOMMENDED_DURATION = 25;
        }

        public override void InitializeDefaults()
        {
            durationValue = Settings.WhiteWineDurationPref;
            base.InitializeDefaults();
        }

        public override void SavePrefs()
        {
            Settings.WhiteWineDurationPref = durationValue;
        }
	}
}
