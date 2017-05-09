using Android.App;
using Android.Content.PM;
using Android.OS;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Vaerator.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.SetTheme(Resource.Style.MyTheme_Main);
            base.OnCreate(bundle);
            Forms.Init(this, bundle);
            LoadApplication(new App());
        }

        // Fix for strange crash which occurs after pressing back button on main page, then navigation back to the application by pressing the launcher icon.
        public override void OnBackPressed()
        {
            var page = Xamarin.Forms.Application.Current.MainPage as Page;

            // Make sure page isn't null and it is last in navigation stack (therefore it's the "home page"). Also ensure there's no modal overlay.
            if (page != null &&
                (
                    !(page is NavigationPage) || (((NavigationPage)page).Navigation.NavigationStack.Count == 1 && ((NavigationPage)page).Navigation.ModalStack.Count == 0)
                ))
                MoveTaskToBack(true);
            else
                base.OnBackPressed();
        }
    }
}