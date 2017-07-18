using System;
using Vaerator.Services;
using Android.Views;
using Android.App;
using Vaerator.Droid.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(RotationService))]
namespace Vaerator.Droid.Services
{
    public class RotationService : IRotationService
    {
        public DeviceRotation GetRotation()
        {
            var surfaceOrientation = ((Activity)Xamarin.Forms.Forms.Context).WindowManager.DefaultDisplay.Rotation;
            switch (surfaceOrientation)
            {
                case SurfaceOrientation.Rotation0:
                    return DeviceRotation.ROTATION0;
                case SurfaceOrientation.Rotation90:
                    return DeviceRotation.ROTATION90;
                case SurfaceOrientation.Rotation180:
                    return DeviceRotation.ROTATION180;
                case SurfaceOrientation.Rotation270:
                    return DeviceRotation.ROTATION270;
                default:
                    return DeviceRotation.ROTATION0;
            }
        }
    }
}