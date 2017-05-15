using System.ComponentModel;
using System.Drawing;
using UIKit;
using Vaerator.Controls;
using Vaerator.iOS.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedFrame), typeof(ExtendedFrameRenderer))]
namespace Vaerator.iOS.Controls
{
    public class ExtendedFrameRenderer : VisualElementRenderer<ExtendedFrame>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ExtendedFrame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
                SetupLayer();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
                e.PropertyName == ExtendedFrame.OutlineColorProperty.PropertyName ||
                e.PropertyName == ExtendedFrame.HasShadowProperty.PropertyName ||
                e.PropertyName == ExtendedFrame.CornerRadiusProperty.PropertyName ||
                e.PropertyName == ExtendedFrame.OutlineWidthProperty.PropertyName )
                SetupLayer();
        }

        void SetupLayer()
        {
            float cornerRadius = Element.CornerRadius;

            if (cornerRadius == -1f)
                cornerRadius = 5f; // default corner radius

            Layer.CornerRadius = cornerRadius;

            if (Element.BackgroundColor == Color.Default)
                Layer.BackgroundColor = UIColor.White.CGColor;
            else
                Layer.BackgroundColor = Element.BackgroundColor.ToCGColor();

            if (Element.HasShadow)
            {
                Layer.ShadowRadius = 5;
                Layer.ShadowColor = UIColor.Black.CGColor;
                Layer.ShadowOpacity = 0.8f;
                Layer.ShadowOffset = new SizeF();
            }
            else
                Layer.ShadowOpacity = 0;

            if (Element.OutlineColor == Color.Default)
                Layer.BorderColor = UIColor.Clear.CGColor;
            else
            {
                Layer.BorderColor = Element.OutlineColor.ToCGColor();
                Layer.BorderWidth = Element.OutlineWidth;
            }

            Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            Layer.ShouldRasterize = true;
        }
    }
}
