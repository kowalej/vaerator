using Xamarin.Forms;
using Vaerator.ViewModels;
using Localization.Localize;
using Localization.TranslationResources;

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

            //Set custom color for red wine.
            fluidColor = Color.FromHex("fb9f58");
            SetupFluidSim(WineContainer, "white_wine_staticbg.jpg");
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
            Title = BeverageResources.WhiteWinePageTitle;
            GlassHereText.Text = BeverageResources.PlaceGlassText;
            DurationLabel.Text = BeverageResources.DurationLabel;
            StartAerateButton.Text = BeverageResources.AerateStartButtonLabel;
            StopAerateButton.Text = BeverageResources.AerateStopButtonLabel;
        }

    }
}
