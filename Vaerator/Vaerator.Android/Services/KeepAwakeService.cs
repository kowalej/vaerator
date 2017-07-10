using Android.App;
using Vaerator.Droid.Services;
using Vaerator.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(KeepAwakeService))]
namespace Vaerator.Droid.Services
{
    public class KeepAwakeService : IKeepAwakeService
    {
        public void StartAwake()
        {
            ((Activity)Xamarin.Forms.Forms.Context).Window.AddFlags(Android.Views.WindowManagerFlags.KeepScreenOn);
        }

        public void StopAwake()
        {
            ((Activity)Xamarin.Forms.Forms.Context).Window.ClearFlags(Android.Views.WindowManagerFlags.KeepScreenOn);
        }
    }
}