using Vaerator.Ads;
using Vaerator.Misc;
using Vaerator.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Vaerator
{
	public partial class App : Application
	{
        public App()
		{
			InitializeComponent();
			SetMainPage();
            #if DEBUG 
                string interstitialAdUnitID = Device.RuntimePlatform == Device.UWP ? UsefulStuff.UWPTest_InterstitialAdUnitID : UsefulStuff.AdMobTest_InterstitialAdUnitID;
            #else
                string interstitialAdUnitID = Device.RuntimePlatform == Device.UWP ? UsefulStuff.UWP_InterstitialAdUnitID : UsefulStuff.AdMob_InterstitialAdUnitID;
            #endif
            CrossInterstitialAdService.Instance.Initialize(interstitialAdUnitID);
		}

		public static void SetMainPage()
		{
            var mainMenuPage = new NavigationPage(new MainMenuPage());
            Current.MainPage = mainMenuPage;
        }
    }
}
