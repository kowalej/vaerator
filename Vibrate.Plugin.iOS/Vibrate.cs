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
            Task.Run(async () =>
            {
                await Task.Delay(milliseconds);
                StopVibration();
            });
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

            if (timeRemaining <= 0)
                return;

            // Vibration time is exactly 500ms on iOS, so for less we stop early, for more we keep calling until <500ms remaining.
            SystemSound.Vibrate.PlaySystemSound();
            await Task.Delay(500, ct);
            await TimedVibrate(timeRemaining - 500);
        }
    }
}