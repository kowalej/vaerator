using Android.Gms.Ads;
using Vaerator.Misc;
using Xamarin.Forms;
using Vaerator.Droid.Ads;
using Vaerator.Ads;

[assembly: Dependency(typeof(InterstitialAdService))]
namespace Vaerator.Droid.Ads
{
    public class InterstitialAdService : IInterstitialAdService
    {
        InterstitialAd interstitialAd;

        public InterstitialAdService(string adUnitID)
        {
            interstitialAd = new InterstitialAd(Android.App.Application.Context);

            // TODO: change this id to your admob id
            interstitialAd.AdUnitId = adUnitID;
            LoadAd();
        }

        void LoadAd()
        {
            var requestbuilder = new AdRequest.Builder();
            interstitialAd.LoadAd(requestbuilder.Build());
        }

        public void ShowAd()
        {
            if (interstitialAd.IsLoaded)
                interstitialAd.Show();

            LoadAd();
        }
    }
}