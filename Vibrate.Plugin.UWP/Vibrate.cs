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
        const int VIBRATE_TIME_MILLIS = 5000; // UWP vibrate max duration.
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
                try
                {
                    Task.Run(() => TimedVibrate(milliseconds));
                    vibrating = true;
                }
                catch (TaskCanceledException ex) { }
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

        async Task TimedVibrate(int timeMillis)
        {
            int fullCycles = timeMillis / VIBRATE_TIME_MILLIS;
            int remainder = timeMillis % VIBRATE_TIME_MILLIS;
            var v = Windows.Phone.Devices.Notification.VibrationDevice.GetDefault();
            for (int i = 0; i < fullCycles; i++)
            {
                if (ct.IsCancellationRequested)
                    return;
                v.Vibrate(TimeSpan.FromMilliseconds(VIBRATE_TIME_MILLIS));
                await Task.Delay(VIBRATE_TIME_MILLIS, ct);
            }
            if (remainder > 0)
            {
                if (ct.IsCancellationRequested)
                    return;
                v.Vibrate(TimeSpan.FromMilliseconds(remainder));
            }
        }
    }
}
