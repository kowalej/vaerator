using Xamarin.Forms;
using Vaerator.ViewModels;
using Localization.Localize;
using Localization.TranslationResources;
using System.Collections.Generic;

namespace Vaerator.Views
{
    public partial class WhiteWinePage : BeverageBasePage
    {
        public WhiteWinePage()
        {
            InitializeComponent();

            startAerateButton = StartAerateButton;
            stopAerateButton = StopAerateButton;
            durationSlider = DurationSlider;
            durationSliderContainer = DurationSliderContainer;

            string funMessageResource = nameof(WhiteWineFun);
            funMessageSource = new Helpers.MessageSource(Localization.Enums.MessageType.FUN, funMessageResource, ResourceContainer.Instance.GetAllResourceKeys(funMessageResource));
            string factMessageResource = nameof(WhiteWineFacts);
            factMessageSource = new Helpers.MessageSource(Localization.Enums.MessageType.FACT, factMessageResource, ResourceContainer.Instance.GetAllResourceKeys(factMessageResource));
            messageBox = Messages;

            vibrationPattern = new List<int>() { 2250, 500, 2250, 500, 2250, 500, 2250, 500, 4200, 2000 };

            //Set custom fluid sim.
            viscosityConstant *= 0.98f;
            fluidColor = Color.FromHex("ffc156");
            SetupFluidSim(WineContainer, "white_wine_staticbg.png");

            vm = (WhiteWineViewModel)BindingContext;
            vm.InitializeDefaults();
            glassHereContainer = GlassHereContainer;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override void SetTranslationText()
        {
            Title = BeverageResources.WhiteWinePageTitle;
            GlassHereText.Text = BeverageResources.PlaceGlassText;
            DurationLabel.Text = BeverageResources.DurationLabel;
            StartAerateButton.Text = BeverageResources.AerateStartButtonLabel;
            StopAerateButton.Text = BeverageResources.AerateStopButtonLabel;
        }

    }
}
