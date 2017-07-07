using System;
using System.Diagnostics;
using Vaerator.UWP.Controls;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ResolutionGroupName(Vaerator.Controls.UnderlineEffect.EffectGroupName)]
[assembly: ExportEffect(typeof(UnderlineEffect), nameof(UnderlineEffect))]
namespace Vaerator.UWP.Controls
{
    public class UnderlineEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            SetUnderline(true);
        }

        protected override void OnDetached()
        {
            SetUnderline(false);
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == Label.TextProperty.PropertyName || args.PropertyName == Label.FormattedTextProperty.PropertyName)
            {
                SetUnderline(true);
            }
        }

        private void SetUnderline(bool underlined)
        {
            try
            {
                var label = (TextBlock)Control;
                Run r = new Run();
                r.Text = label.Text;
                if (underlined)
                {
                    Underline ul = new Underline();
                    ul.Inlines.Add(r);
                    label.Inlines.Clear();
                    label.Inlines.Add(ul);
                }
                else
                {
                    label.Inlines.Clear();
                    label.Inlines.Add(r);
                }
            }
            catch (Exception ex)
            {
                #if DEBUG
                    Debug.WriteLine("Cannot underline Label. Error: ", ex.Message);
                #endif
            }
        }
    }
}
