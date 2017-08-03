using Vaerator.iOS.Services;
using Vaerator.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(KeepAwakeService))]
namespace Vaerator.iOS.Services
{
    public class KeepAwakeService : IKeepAwakeService
    {
        public void StartAwake()
        { 
            UIKit.UIApplication.SharedApplication.IdleTimerDisabled = true;
        }

        public void StopAwake()
        {
            UIKit.UIApplication.SharedApplication.IdleTimerDisabled = false;
        }
    }
}