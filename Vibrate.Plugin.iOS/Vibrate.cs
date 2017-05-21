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
            Task.Run(() => TimedVibrate(milliseconds));
            vibrating = true;
        }

        public void StopVibration()
        {
            if (cts != null)
                cts.Cancel();
            SystemSound.Vibrate.Close();
        }

        async Task TimedVibrate(int timeRemaining)
        {
            if (ct.IsCancellationRequested)
                return;

            if (timeRemaining < 0)
                timeRemaining = 0;

            // Vibration time is exactly 500ms on iOS, so for less we stop early, for more we keep calling until <500ms remaining.
            SystemSound.Vibrate.PlaySystemSound();
            if (timeRemaining <= 500)
            {
                await Task.Delay(timeRemaining, ct);
                SystemSound.Vibrate.Close();
            }
            else
            {
                await Task.Delay(500, ct);
                await TimedVibrate(timeRemaining - 500);
            }
        }
    }
}