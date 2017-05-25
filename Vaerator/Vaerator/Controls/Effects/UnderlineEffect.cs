using Xamarin.Forms;

namespace Vaerator.Controls
{
    public class UnderlineEffect : RoutingEffect
    {
        public const string EffectGroupName = "com.codifferent";
        public UnderlineEffect() : base($"{EffectGroupName}.{nameof(UnderlineEffect)}")
        {

        }
    }
}
