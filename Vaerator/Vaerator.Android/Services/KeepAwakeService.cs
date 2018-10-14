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
            MainActivity.Instance.Window.AddFlags(Android.Views.WindowManagerFlags.KeepScreenOn);
        }

        public void StopAwake()
        {
            MainActivity.Instance.Window.ClearFlags(Android.Views.WindowManagerFlags.KeepScreenOn);
        }
    }
}