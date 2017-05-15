using System;
using System.ComponentModel;
using Vaerator.Controls;
using Vaerator.UWP.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(ExtendedFrame), typeof(ExtendedFrameRenderer))]
namespace Vaerator.UWP.Controls
{
    internal static class ConvertExtensions
    { 
        public static Brush ToBrush(this Color color)
        {
            return new SolidColorBrush(color.ToWindowsColor());
        }

        public static Windows.UI.Color ToWindowsColor(this Color color)
        {
            return Windows.UI.Color.FromArgb((byte)(color.A * 255), (byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
        }
    }

    public class ExtendedFrameRenderer : ViewRenderer<ExtendedFrame, Border>
    {
        Color tempBackgroundColor;

        public ExtendedFrameRenderer()
        {
            AutoPackage = false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ExtendedFrame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                    SetNativeControl(new Border());

                PackChild();
                UpdateBorder();
                UpdateCornerRadius();
                UpdateBorderBackgroundColor();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Content")
            {
                PackChild();
            }
            else if (e.PropertyName == ExtendedFrame.OutlineColorProperty.PropertyName || e.PropertyName == ExtendedFrame.HasShadowProperty.PropertyName)
            {
                UpdateBorder();
            }
            else if (e.PropertyName == ExtendedFrame.CornerRadiusProperty.PropertyName)
            {
                UpdateCornerRadius();
            }
            else if (e.PropertyName == ExtendedFrame.BackgroundColorProperty.PropertyName)
            {
                UpdateBorderBackgroundColor();
            }
        }

        void PackChild()
        {
            if (Element.Content == null)
                return;

            IVisualElementRenderer renderer = Element.Content.GetOrCreateRenderer();
            tempBackgroundColor = Element.BackgroundColor;
            Element.BackgroundColor = new Color(0, 0, 0, 0);
            Control.Child = renderer.ContainerElement;
        }

        void UpdateBorder()
        {
            if (Element.OutlineColor != Color.Default)
            {
                Control.BorderBrush = Element.OutlineColor.ToBrush();
                Control.BorderThickness = new Windows.UI.Xaml.Thickness(Element.OutlineWidth);
            }
            else
            {
                Control.BorderBrush = new Color(0, 0, 0, 0).ToBrush();
            }
        }

        void UpdateCornerRadius()
        {
            float cornerRadius = Element.CornerRadius;

            if (cornerRadius == -1f)
                cornerRadius = 5f; // default corner radius

            Control.CornerRadius = new CornerRadius(cornerRadius);
        }

        void UpdateBorderBackgroundColor()
        {
            Control.Background = tempBackgroundColor.ToBrush();
        }
    }
}
