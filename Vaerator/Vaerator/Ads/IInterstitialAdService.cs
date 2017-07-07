using System;
using System.Collections.Generic;
using System.Text;

namespace Vaerator.Ads
{
    public interface IInterstitialAdService
    {
        void Initialize(string adUnitID);
        void ShowAd();
    }
}
