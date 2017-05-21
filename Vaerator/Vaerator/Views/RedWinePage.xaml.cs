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

            string funMessageResource = nameof(BeverageResources);
            funMessageSource = new Helpers.MessageSource(Localization.Enums.MessageType.FUN, funMessageResource, ResourceContainer.Instance.GetAllResourceKeys(funMessageResource));
            string factMessageResource = nameof(MainMenuResources);
            factMessageSource = new Helpers.MessageSource(Localization.Enums.MessageType.FACT, factMessageResource, ResourceContainer.Instance.GetAllResourceKeys(factMessageResource));
            messageBox = Messages;

            //Set custom color for red wine.
            fluidColor = new Color(0.55f, 0, 0);
            SetupFluidSim(WineContainer, "red_wine_staticbg.jpg");
            vm = (RedWineViewModel)BindingContext;
            vm.InitializeDefaults();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DurationSlider.StyleId = "durationSlider";
        }

        protected override void OnDisappearing()
        {
            vm.SavePrefs();
            base.OnDisappearing();
        }
    }
}
