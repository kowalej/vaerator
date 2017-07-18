using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Gms.Ads;
using Android.OS;
using FFImageLoading.Forms.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Vaerator.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.SetTheme(Resource.Style.MyTheme_Main);
            base.OnCreate(bundle);
            Forms.Init(this, bundle);
            //MobileAds.Initialize(ApplicationContext, Misc.UsefulStuff.AdMob_AppID); // Not deprecated - should be called!
            CachedImageRenderer.Init(); // Enable FFImageLoading
            LoadApplication(new App());
            LockOrientation(); 
        }

        // Fix for strange crash which occurs after pressing back button on main page, then navigation back to the application by pressing the launcher icon.
        public override void OnBackPressed()
        {
            Page page = Xamarin.Forms.Application.Current.MainPage;

            // Make sure page isn't null and it is last in navigation stack (therefore it's the "home page"). Also ensure there's no modal overlay.
            if (page != null &&
                (
                    !(page is NavigationPage) || (((NavigationPage)page).Navigation.NavigationStack.Count == 1 && ((NavigationPage)page).Navigation.ModalStack.Count == 0)
                ))
                MoveTaskToBack(true);
            else
                base.OnBackPressed();
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            newConfig.Orientation = Orientation.Portrait; 
            base.OnConfigurationChanged(newConfig);
        }

        // Lock screen orientation based on device type.
        void LockOrientation()
        {
            RequestedOrientation = ScreenOrientation.Portrait;
        }
    }
}