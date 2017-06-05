using Google.MobileAds;
using UIKit;
using Vaerator.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BannerAd), typeof(Vaerator.iOS.Controls.BannerAdRenderer))]
namespace Vaerator.iOS.Controls
{
    public class BannerAdRenderer : ViewRenderer<BannerAd, BannerView>
    {
        // Create new ad.
        BannerView CreateAdControl(string adUnitID, AdSize adSize)
        {
            BannerView adView = new BannerView();
            adView.AdSize = adSize;
            adView.AdUnitID = adUnitID;
            foreach (UIWindow uiWindow in UIApplication.SharedApplication.Windows)
            {
                if (uiWindow.RootViewController != null)
                {
                    adView.RootViewController = uiWindow.RootViewController;
                }
            }
            adView.LoadRequest(Request.GetDefaultRequest());
            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Vaerator.Controls.BannerAd> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                BannerAd element = Element as BannerAd;
                AdSize adSize;

                switch (element.AdSize)
                {
                    case BannerAd.AdSizes.StandardBanner:
                        adSize = AdSizeCons.Banner;
                        break;
                    case BannerAd.AdSizes.LargeBanner:
                        adSize = AdSizeCons.LargeBanner;
                        break;
                    case BannerAd.AdSizes.MediumRectangle:
                        adSize = AdSizeCons.MediumRectangle;
                        break;
                    case BannerAd.AdSizes.FullBanner:
                        adSize = AdSizeCons.FullBanner;
                        break;
                    case BannerAd.AdSizes.Leaderboard:
                        adSize = AdSizeCons.Leaderboard;
                        break;
                    case BannerAd.AdSizes.SmartBanner:
                        var currentOrientation = UIApplication.SharedApplication.StatusBarOrientation;
                        if (currentOrientation == UIInterfaceOrientation.Portrait || currentOrientation == UIInterfaceOrientation.PortraitUpsideDown)
                        {
                            adSize = AdSizeCons.SmartBannerPortrait;
                        }
                        else adSize = AdSizeCons.SmartBannerLandscape;
                        break;
                    default:
                        adSize = AdSizeCons.Banner;
                        break;
                }
                SetNativeControl(CreateAdControl(element.AdUnitID, adSize));
            }
        }
    }
}
