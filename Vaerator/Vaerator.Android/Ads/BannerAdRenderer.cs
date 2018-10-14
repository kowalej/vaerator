using Android.Widget;
using Android.Gms.Ads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Vaerator.Controls;

[assembly: ExportRenderer(typeof(BannerAd), typeof(Vaerator.Droid.Controls.BannerAdRenderer))]
namespace Vaerator.Droid.Controls
{
    public class BannerAdRenderer : ViewRenderer<BannerAd, AdView>
    {
        public BannerAdRenderer() : base(MainActivity.Instance) { }
        // Create new ad.
        AdView CreateAdControl(string adUnitID, AdSize adSize)
        {
            AdView adView = new AdView(Context);
            adView.AdSize = adSize;
            adView.AdUnitId = adUnitID;
            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            adView.LayoutParameters = adParams;
            //string deviceID = Settings.Secure.GetString(Forms.Context.ContentResolver, Settings.Secure.AndroidId);
            var adRequestBuilder = new AdRequest.Builder();
            //adRequestBuilder.AddTestDevice(deviceID);
            adView.LoadAd(adRequestBuilder.Build());
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
                        adSize = AdSize.Banner;
                        break;
                    case BannerAd.AdSizes.LargeBanner:
                        adSize = AdSize.LargeBanner;
                        break;
                    case BannerAd.AdSizes.MediumRectangle:
                        adSize = AdSize.MediumRectangle;
                        break;
                    case BannerAd.AdSizes.FullBanner:
                        adSize = AdSize.FullBanner;
                        break;
                    case BannerAd.AdSizes.Leaderboard:
                        adSize = AdSize.Leaderboard;
                        break;
                    case BannerAd.AdSizes.SmartBanner:
                        adSize = AdSize.SmartBanner;
                        break;
                    default:
                        adSize = AdSize.Banner;
                        break;
                }
                SetNativeControl(CreateAdControl(element.AdUnitID, adSize));
            }
        }
    }
}