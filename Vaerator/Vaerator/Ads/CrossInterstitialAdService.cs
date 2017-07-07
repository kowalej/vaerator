using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Vaerator.Ads
{
    public class CrossInterstitialAdService
    {
        private static volatile IInterstitialAdService instance;
        private static object syncRoot = new Object();

        public static IInterstitialAdService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = DependencyService.Get<IInterstitialAdService>();
                    }
                }
                return instance;
            }
        }
    }
}
