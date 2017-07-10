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
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            CachedImageRenderer.Init(); // Enable FFImageLoading
            LoadApplication(new Vaerator.App());
        }
    }
}
