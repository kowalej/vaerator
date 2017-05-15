using Xamarin.Forms;

namespace Vaerator.Controls
{
    public class UnderlineEffect : RoutingEffect
    {
        public const string EffectGroupName = "com.stoicdevs";
        public UnderlineEffect() : base($"{EffectGroupName}.{nameof(UnderlineEffect)}")
        {

        }
    }
}
