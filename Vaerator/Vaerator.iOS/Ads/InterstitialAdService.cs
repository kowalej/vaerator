using System;
using UIKit;
using Xamarin.Forms;
using Vaerator.Ads;
using Google.MobileAds;
using Vaerator.iOS.Ads;
using Foundation;
using Vaerator.Misc;

[assembly: Dependency(typeof(InterstitialAdService)), Preserve(AllMembers = true)]
namespace Vaerator.iOS.Ads
{
    public class InterstitialAdService : IInterstitialAdService
    {
        Interstitial interstitialAd;
        Request adRequest;
        string adUnitID;

        public InterstitialAdService() { }

        public void Initialize(string adUnitID)
        {
            if (interstitialAd == null)
            {
                this.adUnitID = adUnitID;
                adRequest = Request.GetDefaultRequest();
                RefreshAd();
            }
        }

        void CreateInterstitial()
        {
            interstitialAd = new Interstitial(adUnitID);
            interstitialAd.ScreenDismissed += (s, e) => RefreshAd();
        }

        void RefreshAd()
        {
            CreateInterstitial();
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