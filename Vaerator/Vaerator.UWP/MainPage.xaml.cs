using FFImageLoading.Forms.WinUWP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms;

namespace Vaerator.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            // Lock screen orientation based on device type.
            if (Device.Idiom == TargetIdiom.Phone)
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            else
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;

            // Enable FFImageLoading
            CachedImageRenderer.Init(); 

            LoadApplication(new Vaerator.App());
        }
    }
}
