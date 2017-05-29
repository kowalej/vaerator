using Xamarin.Forms;
using Vaerator.ViewModels;
using Localization.Localize;
using Localization.TranslationResources;

namespace Vaerator.Views
{
    public partial class RedWinePage : BeverageBasePage
    {
        public RedWinePage()
        {
            InitializeComponent();

            startAerateButton = StartAerateButton;
            stopAerateButton = StopAerateButton;
            durationSlider = DurationSlider;
            durationSliderContainer = DurationSliderContainer;

            string funMessageResource = nameof(RedWineFun);
            funMessageSource = new Helpers.MessageSource(Localization.Enums.MessageType.FUN, funMessageResource, ResourceContainer.Instance.GetAllResourceKeys(funMessageResource));
            string factMessageResource = nameof(RedWineFacts);
            factMessageSource = new Helpers.MessageSource(Localization.Enums.MessageType.FACT, factMessageResource, ResourceContainer.Instance.GetAllResourceKeys(factMessageResource));
            messageBox = Messages;

            //Set custom fluid sim.
            fluidColor = Color.FromHex("8f0009"); // new Color(0.55f, 0, 0);
            SetupFluidSim(WineContainer, "red_wine_staticbg.png");

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
            Title = BeverageResources.RedWinePageTitle;
            GlassHereText.Text = BeverageResources.PlaceGlassText;
            DurationLabel.Text = BeverageResources.DurationLabel;
            StartAerateButton.Text = BeverageResources.AerateStartButtonLabel;
            StopAerateButton.Text = BeverageResources.AerateStopButtonLabel;
        }

    }
}
