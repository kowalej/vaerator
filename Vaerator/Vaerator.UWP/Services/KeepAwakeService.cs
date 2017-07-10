using System;
using Vaerator.Services;
using Vaerator.UWP.Services;
using Windows.System.Display;
using Xamarin.Forms;

[assembly: Dependency(typeof(KeepAwakeService))]
namespace Vaerator.UWP.Services
{
    public class KeepAwakeService : IKeepAwakeService
    {
        private DisplayRequest displayRequest;

        public void StartAwake()
        {
            if(displayRequest == null)
                displayRequest = new DisplayRequest();
            displayRequest.RequestActive();
        }

        public void StopAwake()
        {
            if (displayRequest == null)
                return;
            displayRequest.RequestRelease();
        }
    }
}