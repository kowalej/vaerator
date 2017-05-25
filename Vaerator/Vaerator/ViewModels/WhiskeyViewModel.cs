using Localization.TranslationResources;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Vaerator.ViewModels
{
	public class WhiskeyViewModel : BeverageBaseViewModel
	{
        public WhiskeyViewModel()
		{
            DURATION_MIN = 15;
            DURATION_MAX = 45;
            DURATION_STEP_SIZE = 5;
            RECOMMENDED_DURATION = 25;
        }

        public override void InitializeDefaults()
        {
            durationValue = Settings.WhiskeySpiritsDurationPref;
            base.InitializeDefaults();
        }

        public override void SavePrefs()
        {
            Settings.WhiskeySpiritsDurationPref = durationValue;
        }
	}
}
