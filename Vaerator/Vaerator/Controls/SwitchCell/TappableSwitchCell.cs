using Xamarin.Forms;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Vaerator.Controls
{
    public sealed class TappableSwitchCell : SwitchCell
    {
        protected override void OnTapped()
        {
            base.OnTapped();
            On = On ? false : true;
        }
    }
}
