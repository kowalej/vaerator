using FFImageLoading.Forms.Touch;
using Foundation;
using Google.MobileAds;
using UIKit;

namespace Vaerator.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();
            MobileAds.Configure(Misc.UsefulStuff.AdMob_iOS_AppID); // Not deprecated - should be called!
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
