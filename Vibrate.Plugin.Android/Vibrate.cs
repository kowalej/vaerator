using System;

using Plugin.Vibrate.Abstractions;
using Android.OS;
using Android.Content;
using Android.Media;

namespace Plugin.Vibrate
{
    /// <summary>
    /// Vibration Implentation on Android 
    /// </summary>
    public class Vibrate : IVibrate
    {
        /// <summary>
        /// Vibrate device for specified amount of time
        /// </summary>
        /// <param name="milliseconds">Time in MS to vibrate device (500ms is default).</param>
        public void StartVibration(int milliseconds = 500)
        {
            using (var v = (Vibrator)Android.App.Application.Context.GetSystemService(Context.VibratorService))
            {
                if ((int)Build.VERSION.SdkInt >= 11)
                {
                    #if __ANDROID_11__
                        if (!v.HasVibrator)
                        {
                            #if DEBUG
                                System.Diagnostics.Debug.WriteLine("Android device does not have vibrator.");
                            #endif
                            return;
                        }
                    #endif
                    }

                if (milliseconds < 0)
                    milliseconds = 0;

                try
                {
                    using (var attributes = new Android.Media.AudioAttributes.Builder().SetUsage(Android.Media.AudioUsageKind.Game).Build())
                        v.Vibrate(milliseconds, attributes);
                }
                catch (Exception ex)
                {
                    #if DEBUG
                        System.Diagnostics.Debug.WriteLine("Unable to vibrate Android device, ensure VIBRATE permission is set.");
                    #endif
                }
            }
        }

        public void StopVibration()
        {
            using (var v = (Vibrator)Android.App.Application.Context.GetSystemService(Context.VibratorService))
            {
                v.Cancel();
            }
        }
    }
}