using Microsoft.Advertising.WinRT.UI;
using Vaerator.Ads;
using Vaerator.Misc;
using Vaerator.UWP.Ads;
using Xamarin.Forms;

[assembly: Dependency(typeof(InterstitialAdService))]
namespace Vaerator.UWP.Ads
{
    public class InterstitialAdService : IInterstitialAdService
    {
        InterstitialAd interstitialAd;
        string applicationID = null;
        string adUnitID = null;

        public void Initialize(string adUnitID)
        {
            interstitialAd = new InterstitialAd();
            applicationID = UsefulStuff.UWP_AdAppID;
            this.adUnitID = adUnitID;
            interstitialAd.Completed += (s, e) => RefreshAd();
            RefreshAd();
        }

        void RefreshAd()
        {
            interstitialAd.RequestAd(AdType.Display, applicationID, adUnitID);
        }

        public void ShowAd()
        {
            if (InterstitialAdState.Ready == interstitialAd.State)
                interstitialAd.Show();
        }
    }
}
