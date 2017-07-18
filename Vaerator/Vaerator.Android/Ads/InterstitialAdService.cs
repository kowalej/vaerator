using Android.Gms.Ads;
using Xamarin.Forms;
using Vaerator.Droid.Ads;
using Vaerator.Ads;
using System;

[assembly: Dependency(typeof(InterstitialAdService))]
namespace Vaerator.Droid.Ads
{
    public class AdListenerD : AdListener
    {
        Action onClosed;
        public Action OnClosed { get { return onClosed; } set { onClosed = value; } }

        public AdListenerD()
        {

        }

        public override void OnAdClosed()
        {
            base.OnAdClosed();
            onClosed();
        }
    }

    public class InterstitialAdService : IInterstitialAdService
    {
        InterstitialAd interstitialAd;
        AdRequest adRequest;

        public InterstitialAdService() { }

        public void Initialize(string adUnitID)
        {
            if (interstitialAd == null)
            {
                interstitialAd = new InterstitialAd(Android.App.Application.Context);
                interstitialAd.AdUnitId = adUnitID;
                adRequest = new AdRequest.Builder().Build();
                AdListenerD adListener = new AdListenerD();
                adListener.OnClosed = () => RefreshAd();
                interstitialAd.AdListener = adListener;
                RefreshAd();
            }
        }

        public void RefreshAd()
        {
            interstitialAd.LoadAd(adRequest);
        }

        public void ShowAd()
        {
            if (interstitialAd == null) throw new Exception("Cannot show ad, interstitial not initialized. Please call Initialize(adUnitID) before showing!");
            if (interstitialAd.IsLoaded)
                interstitialAd.Show();
        }
    }
}