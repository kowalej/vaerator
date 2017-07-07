using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Windows.UI.Xaml;
using Windows.System.Profile;
using Vaerator.Controls;
using Microsoft.Advertising.WinRT.UI;
using static Vaerator.Controls.BannerAd;
using Vaerator.Misc;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(BannerAd), typeof(Vaerator.UWP.Controls.BannerAdRenderer))]
namespace Vaerator.UWP.Controls
{
    public class BannerAdRenderer : ViewRenderer<View, FrameworkElement>
    {
        AdControl bannerView;

        void CreateAdControl(AdSizes adSize, string adUnitID)
        {
            bannerView = new AdControl();
            switch (adSize)
            {
                case AdSizes.StandardBanner:
                    bannerView.Width = 320;
                    bannerView.Height = 50;
                    break;
                case AdSizes.LargeBanner:
                    bannerView.Width = 320;
                    bannerView.Height = 50;
                    break;
                case AdSizes.MediumRectangle:
                    bannerView.Width = 300;
                    bannerView.Height = 250;
                    break;
                case AdSizes.FullBanner:
                    bannerView.Width = 480;
                    bannerView.Height = 80;
                    break;
                case AdSizes.Leaderboard:
                    bannerView.Width = 728;
                    bannerView.Height = 90;
                    break;
                case AdSizes.SmartBanner:
                    bannerView.Width = 320;
                    bannerView.Height = 50;
                    break;
                default:
                    bannerView.Width = 320;
                    bannerView.Height = 50;
                    break;
            }

            bannerView.ApplicationId = UsefulStuff.UWP_AdAppID;
            bannerView.AdUnitId = adUnitID;
            bannerView.AutoRefreshIntervalInSeconds = 6;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            BannerAd element = sender as BannerAd;
            if (e.PropertyName == BannerAd.IsVisibleProperty.PropertyName && element.IsVisible == true)
            {
                bannerView = null;
                UpdateNativeControl();
                CreateAdControl(element.AdSize, element.AdUnitID);
                SetNativeControl(bannerView);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                BannerAd element = Element as BannerAd;
                CreateAdControl(element.AdSize, element.AdUnitID);
                if (bannerView != null)
                    SetNativeControl(bannerView);
            }
        }
    }
}