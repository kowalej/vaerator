using Plugin.Vibrate.Abstractions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace Plugin.Vibrate
{
    public class Vibrate : IVibrate
    {
        bool vibrating = false;
        CancellationTokenSource cts;
        CancellationToken ct;

        public void StartVibration(int milliseconds = 500)
        {
            if (ApiInformation.IsTypePresent("Windows.Phone.Devices.Notification.VibrationDevice"))
            {
                if (vibrating)
                    StopVibration();
                cts = new CancellationTokenSource();
                ct = cts.Token;
                Task.Run(() => TimedVibrate(milliseconds));
                vibrating = true;
            }
            else
            {
                #if DEBUG
                    Debug.WriteLine("Vibration not supported on this device family.");
                #endif
            }
        }

        public void StopVibration()
        {
            if (cts != null)
                cts.Cancel();
            if (ApiInformation.IsTypePresent("Windows.Phone.Devices.Notification.VibrationDevice"))
                Windows.Phone.Devices.Notification.VibrationDevice.GetDefault().Cancel();
            else
            {
                #if DEBUG
                    Debug.WriteLine("Vibration not supported on this device family.");
                #endif
            }
        }

        async Task TimedVibrate(int timeRemaining)
        {
            if (ct.IsCancellationRequested)
                return;

            if (timeRemaining < 0)
                timeRemaining = 0;

            // Windows only allows 5000ms vibration, so if we want to do more we have to restart.
            var v = Windows.Phone.Devices.Notification.VibrationDevice.GetDefault();
            if (timeRemaining > 5000)
            {
                v.Vibrate(TimeSpan.FromMilliseconds(5000));
                await Task.Delay(5000, ct);
                await TimedVibrate(timeRemaining - 5000);
            }
            else v.Vibrate(TimeSpan.FromMilliseconds(timeRemaining));
        }
    }
}
