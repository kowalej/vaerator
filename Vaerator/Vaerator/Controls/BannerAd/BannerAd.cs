using Vaerator.Misc;
using Xamarin.Forms;

namespace Vaerator.Controls
{
    public class BannerAd : View
    {
        public enum AdSizes { SmartBanner, StandardBanner, LargeBanner, MediumRectangle, FullBanner, Leaderboard }
        AdSizes adSize = default(AdSizes);
        public AdSizes AdSize { get { return adSize; } set { adSize = value; } }
        
        string adUnitID = UsefulStuff.AdMobTest_BannerAdUnitID;
        public string AdUnitID { get { return adUnitID; } set { adUnitID = value; } }
    }
}
