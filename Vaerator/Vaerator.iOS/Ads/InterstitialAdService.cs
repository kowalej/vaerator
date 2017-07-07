using System;
using UIKit;
using Xamarin.Forms;
using Vaerator.Ads;
using Google.MobileAds;
using Vaerator.iOS.Ads;

[assembly: Dependency(typeof(InterstitialAdService))]
namespace Vaerator.iOS.Ads
{
    public class InterstitialAdService : IInterstitialAdService
    {
        Interstitial interstitialAd;
        Request adRequest; 

        public InterstitialAdService()
        {
            interstitialAd.ScreenDismissed += (s, e) => RefreshAd();
        }

        public void Initialize(string adUnitID)
        {
            interstitialAd = new Interstitial(adUnitID);
            adRequest = Request.GetDefaultRequest();
            RefreshAd();
        }

        void RefreshAd()
        {
            interstitialAd.LoadRequest(adRequest);
        }

        public void ShowAd()
        {
            if (interstitialAd == null) throw new Exception("Cannot show ad, interstitial not initialized. Please call Initialize(adUnitID) before showing!");
            if (interstitialAd.IsReady)
            {
                var viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                interstitialAd.PresentFromRootViewController(viewController);
            }
        }
    }
}