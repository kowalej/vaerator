using FFImageLoading.Forms.Touch;
using Foundation;
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

            // Enable FFImageLoading
            CachedImageRenderer.Init(); 

            LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}

        // Lock screen orientation based on device type.
        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
        {
            if (Device.Idiom == TargetIdiom.Tablet)
                return UIInterfaceOrientationMask.Landscape;
            else
                return UIInterfaceOrientationMask.Portrait;
        }
    }
}
