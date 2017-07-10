using FFImageLoading.Forms.Touch;
using Foundation;
using Google.MobileAds;
using UIKit;
using Xamarin.Forms;

namespace Vaerator.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();
            var poop = typeof(Vaerator.iOS.Ads.InterstitialAdService);
            var shit = typeof(Google.MobileAds.Interstitial);
            MobileAds.Configure(Misc.UsefulStuff.AdMob_AppID); // Not deprecated - should be called!
            CachedImageRenderer.Init(); // Enable FFImageLoading
            LoadApplication(new App());
			return base.FinishedLaunching(app, options);
		}

        // Lock screen orientation based on device type.
        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
        {
            return UIInterfaceOrientationMask.Portrait;
        }
    }
}
