using FFImageLoading.Forms.WinUWP;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Xamarin.Forms;

namespace Vaerator.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(1024, 1024));

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
