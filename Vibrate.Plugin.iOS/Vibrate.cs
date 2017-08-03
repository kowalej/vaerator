using Plugin.Vibrate.Abstractions;
using AudioToolbox;
using AVFoundation;
using System.Threading.Tasks;
using System.Threading;

namespace Plugin.Vibrate
{
    /// <summary>
    /// iOS implementation to vibrate device
    /// </summary>
    public class Vibrate : IVibrate
    {
        const int VIBRATE_TIME_MILLIS = 500; // iOS vibrate duration.
        bool vibrating = false;
        CancellationTokenSource cts;
        CancellationToken ct; 

        /// <summary>
        /// Vibrate device with default length
        /// </summary>
        /// <param name="milliseconds">Time in MS to vibrate device (500ms is default).</param>
        public void StartVibration(int milliseconds = 500)
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
            catch (TaskCanceledException) { }
        }

        public void StopVibration()
        {
            if (cts != null)
                cts.Cancel();
            SystemSound.Vibrate.Close();
        }

        async Task TimedVibrate(int timeMillis)
        {
            int fullCycles = timeMillis / VIBRATE_TIME_MILLIS;
            int remainder = timeMillis % VIBRATE_TIME_MILLIS;
            for (int i = 0; i < fullCycles; i++)
            {
                if (ct.IsCancellationRequested)
                    return;
                SystemSound.Vibrate.PlaySystemSound();
                await Task.Delay(VIBRATE_TIME_MILLIS, ct);
            }
            if(remainder > 0)
            {
                if (ct.IsCancellationRequested)
                    return;
                SystemSound.Vibrate.PlaySystemSound();
                await Task.Delay(remainder, ct);
                SystemSound.Vibrate.Close();
            }
        }
    }
}