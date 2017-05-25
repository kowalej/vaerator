using Xamarin.Forms;
using Vaerator.ViewModels;
using Localization.Localize;
using Localization.TranslationResources;
using System;

namespace Vaerator.Views
{
    public partial class WhiskeyPage : BeverageBasePage
    {
        public WhiskeyPage()
        {
            InitializeComponent();

            startAerateButton = StartAerateButton;
            stopAerateButton = StopAerateButton;
            durationSlider = DurationSlider;
            durationSliderContainer = DurationSliderContainer;

            string funMessageResource = nameof(WhiskeySpiritFun);
            funMessageSource = new Helpers.MessageSource(Localization.Enums.MessageType.FUN, funMessageResource, ResourceContainer.Instance.GetAllResourceKeys(funMessageResource));
            string factMessageResource = nameof(WhiskeySpiritFacts);
            factMessageSource = new Helpers.MessageSource(Localization.Enums.MessageType.FACT, factMessageResource, ResourceContainer.Instance.GetAllResourceKeys(factMessageResource));
            messageBox = Messages;

            //Set custom color for red wine.
            fluidColor = new Color(0.55f, 0, 0);
            SetupFluidSim(WineContainer, "whiskey_staticbg.jpg");
            vm = (RedWineViewModel)BindingContext;
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
            Title = BeverageResources.WhiskeyPageTitle;
            GlassHereText.Text = BeverageResources.PlaceGlassText;
            DurationLabel.Text = BeverageResources.DurationLabel;
            StartAerateButton.Text = BeverageResources.AerateStartButtonLabel;
            StopAerateButton.Text = BeverageResources.AerateStopButtonLabel;
        }

    }
}
