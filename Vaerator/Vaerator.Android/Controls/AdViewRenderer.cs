using Android.Widget;
using Android.Gms.Ads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Vaerator.Controls.AdView), typeof(Vaerator.Droid.Controls.AdViewRenderer))]
namespace Vaerator.Droid.Controls
{
    public class AdViewRenderer : ViewRenderer<Vaerator.Controls.AdView, AdView>
    {
        string adUnitId = string.Empty;
        AdSize adSize = AdSize.SmartBanner;
        AdView adView;
        AdView CreateNativeControl()
        {
            if (adView != null)
                return adView;

            adUnitId = Forms.Context.Resources.GetString(Resource.String.banner_ad_unit_id);
            adView = new AdView(Forms.Context);
            adView.AdSize = adSize;
            adView.AdUnitId = adUnitId;

            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            adView.LayoutParameters = adParams;

            adView.LoadAd(new AdRequest
                            .Builder()
                            .Build());
            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Vaerator.Controls.AdView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                CreateNativeControl();
                SetNativeControl(adView);
            }
        }
    }
}