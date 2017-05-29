using Xamarin.Forms;
using Vaerator.ViewModels;
using Localization.Localize;
using Localization.TranslationResources;
using System;
using System.Collections.Generic;

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

            vibrationPattern = new List<int>() { 5000, 500, 5000, 500 };

            //Set custom fluid sim.
            viscosityConstant *= 1.04f;
            fluidColor = Color.FromHex("b46100");
            SetupFluidSim(WineContainer, "whiskey_staticbg.png");

            vm = (WhiskeyViewModel)BindingContext;
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
