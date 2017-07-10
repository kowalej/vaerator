using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Vaerator.Enums;
using Vaerator.Controls;

[assembly: ExportRenderer(typeof(ImageButton), typeof(ImageButtonRenderer))]
namespace Vaerator.Controls
{
    /// <summary>
    /// Draws a button on the iOS platform with the image shown in the right 
    /// position with the right size.
    /// </summary>
    public partial class ImageButtonRenderer : ButtonRenderer
    {
        /// <summary>
        /// The padding to use in the control.
        /// </summary>
        private const int CONTROL_PADDING = 2;

        /// <summary>
        /// Identifies the iPad.
        /// </summary>
        private const string IPAD = "iPad";

        /// <summary>
        /// Gets the underlying element typed as an <see cref="ImageButton"/>.
        /// </summary>
        private ImageButton ImageButton
        {
            get { return (ImageButton)Element; }
        }

        /// <summary>
        /// Handles the initial drawing of the button.
        /// </summary>
        /// <param name="e">Information on the <see cref="ImageButton"/>.</param> 
        protected override async void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            var imageButton = ImageButton;
            var targetButton = Control;
            if (imageButton != null && targetButton != null && imageButton.Source != null)
            {
                // Matches Android ImageButton behavior
                targetButton.LineBreakMode = UIKit.UILineBreakMode.WordWrap;

                var width = imageButton.ImageWidthRequest;
                var height = imageButton.ImageHeightRequest;

                await SetupImages(imageButton, targetButton, width, height);

                switch (imageButton.Orientation)
                {
                    case ImageOrientation.ImageToLeft:
                        AlignToLeft(targetButton);
                        break;
                    case ImageOrientation.ImageToRight:
                        AlignToRight(imageButton.ImageWidthRequest, targetButton);
                        break;
                    case ImageOrientation.ImageOnTop:
                        AlignToTop(imageButton.ImageHeightRequest, imageButton.ImageWidthRequest, targetButton);
                        break;
                    case ImageOrientation.ImageOnBottom:
                        AlignToBottom(imageButton.ImageHeightRequest, imageButton.ImageWidthRequest, targetButton);
                        break;
                    case ImageOrientation.ImageCenterToLeft:
                        AlignToCenter(targetButton, imageButton.ImageWidthRequest, true);
                        break;
                    case ImageOrientation.ImageCenterToRight:
                        AlignToCenter(targetButton, imageButton.ImageWidthRequest, false);
                        break;
                }
            }
        }

        /// <summary>
        /// Called when the underlying model's properties are changed.
        /// </summary>
        /// <param name="sender">Model sending the change event.</param>
        /// <param name="e">Event arguments.</param>
        protected async override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == ImageButton.SourceProperty.PropertyName ||
                e.PropertyName == ImageButton.DisabledSourceProperty.PropertyName ||
                e.PropertyName == ImageButton.ImageTintColorProperty.PropertyName ||
                e.PropertyName == ImageButton.DisabledImageTintColorProperty.PropertyName)
            {
                var sourceButton = Element as ImageButton;
                if (sourceButton != null && sourceButton.Source != null)
                {
                    var imageButton = ImageButton;
                    var targetButton = Control;
                    if (imageButton != null && targetButton != null && imageButton.Source != null)
                    {
                        await SetupImages(imageButton, targetButton, imageButton.ImageWidthRequest, imageButton.ImageHeightRequest);
                    }
                }
            }
        }

        async Task SetupImages(ImageButton imageButton, UIButton targetButton, double width, double height)
        {
            UIColor tintColor = imageButton.ImageTintColor == Color.Transparent ? null : imageButton.ImageTintColor.ToUIColor();
            UIColor disabledTintColor = imageButton.DisabledImageTintColor == Color.Transparent ? null : imageButton.DisabledImageTintColor.ToUIColor();

            await SetImageAsync(imageButton.Source, width, height, targetButton, UIControlState.Normal, tintColor);

            if (imageButton.DisabledSource != null || disabledTintColor != null)
            {
                await SetImageAsync(imageButton.DisabledSource ?? imageButton.Source, width, height, targetButton, UIControlState.Disabled, disabledTintColor);
            }
        }

        /// <summary>
        /// Properly aligns the title and image on a button to the left.
        /// </summary>
        /// <param name="targetButton">The button to align.</param>
        private static void AlignToLeft(UIButton targetButton)
        {
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Left;

            var titleInsets = new UIEdgeInsets(0, CONTROL_PADDING, 0, -1 * CONTROL_PADDING);
            targetButton.TitleEdgeInsets = titleInsets;
        }

        /// <summary>
        /// Properly aligns the title and image on a button to the right.
        /// </summary>
        /// <param name="widthRequest">The requested image width.</param>
        /// <param name="targetButton">The button to align.</param>
        private static void AlignToRight(double widthRequest, UIButton targetButton)
        {
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Right;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Right;

            var titleInsets = new UIEdgeInsets(0, 0, 0, (nfloat)widthRequest + CONTROL_PADDING);

            targetButton.TitleEdgeInsets = titleInsets;
            var imageInsets = new UIEdgeInsets(0, (nfloat)widthRequest, 0, -1 * (nfloat)widthRequest);
            targetButton.ImageEdgeInsets = imageInsets;
        }

        /// <summary>
        /// Properly aligns the title and image on a button when the image is over the title.
        /// </summary>
        /// <param name="heightRequest">The requested image height.</param>
        /// <param name="widthRequest">The requested image width.</param>
        /// <param name="targetButton">The button to align.</param>
        private static void AlignToTop(double heightRequest, double widthRequest, UIButton targetButton)
        {
            targetButton.VerticalAlignment = UIControlContentVerticalAlignment.Top;
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Center;
            targetButton.TitleLabel.LineBreakMode = UIKit.UILineBreakMode.WordWrap;

            targetButton.SizeToFit();

            var titleWidth = targetButton.TitleLabel.IntrinsicContentSize.Width;
            CGSize titleSize = targetButton.TitleLabel.Frame.Size;

            UIEdgeInsets titleInsets;
            UIEdgeInsets imageInsets;

            if (UIDevice.CurrentDevice.Model.Contains(IPAD))
            {
                titleInsets = new UIEdgeInsets((nfloat)heightRequest, Convert.ToInt32(-1 * widthRequest / 2), -1 * (nfloat)heightRequest, Convert.ToInt32(widthRequest / 2));
                imageInsets = new UIEdgeInsets(0, Convert.ToInt32(titleWidth / 2), 0, -1 * Convert.ToInt32(titleWidth / 2));
            }
            else
            {
                titleInsets = new UIEdgeInsets((nfloat)heightRequest, Convert.ToInt32(-1 * widthRequest / 2), -1 * (nfloat)heightRequest, Convert.ToInt32(widthRequest / 2));
                imageInsets = new UIEdgeInsets(0, Convert.ToInt32(targetButton.IntrinsicContentSize.Width / 2 - widthRequest / 2 - titleSize.Width / 2), 0, Convert.ToInt32(-1 * (targetButton.IntrinsicContentSize.Width / 2 - widthRequest / 2 + titleSize.Width / 2)));
            }

            targetButton.TitleEdgeInsets = titleInsets;
            targetButton.ImageEdgeInsets = imageInsets;
        }

        /// <summary>
        /// Properly aligns the title and image on a button when the title is over the image.
        /// </summary>
        /// <param name="heightRequest">The requested image height.</param>
        /// <param name="widthRequest">The requested image width.</param>
        /// <param name="targetButton">The button to align.</param>
        private static void AlignToBottom(double heightRequest, double widthRequest, UIButton targetButton)
        {
            targetButton.VerticalAlignment = UIControlContentVerticalAlignment.Bottom;
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Center;
            targetButton.SizeToFit();

            var titleWidth = targetButton.TitleLabel.IntrinsicContentSize.Width;

            UIEdgeInsets titleInsets;
            UIEdgeInsets imageInsets;

            if (UIDevice.CurrentDevice.Model.Contains(IPAD))
            {
                titleInsets = new UIEdgeInsets(-1 * (nfloat)heightRequest, Convert.ToInt32(-1 * widthRequest / 2), (nfloat)heightRequest, Convert.ToInt32(widthRequest / 2));
                imageInsets = new UIEdgeInsets(0, titleWidth / 2, 0, -1 * titleWidth / 2);
            }
            else
            {
                titleInsets = new UIEdgeInsets(-1 * (nfloat)heightRequest, -1 * (nfloat)widthRequest, (nfloat)heightRequest, (nfloat)widthRequest);
                imageInsets = new UIEdgeInsets(0, 0, 0, 0);
            }

            targetButton.TitleEdgeInsets = titleInsets;
            targetButton.ImageEdgeInsets = imageInsets;
        }

        private static void AlignToCenter(UIButton targetButton, double widthRequest, bool left = true)
        {
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Center;

            targetButton.SizeToFit();

            UIEdgeInsets titleInsets;
            UIEdgeInsets imageInsets;

            titleInsets = new UIEdgeInsets(0, -(nfloat)widthRequest, -25, 0);
            imageInsets = new UIEdgeInsets(-15.0f, 0.0f, 0.0f, -targetButton.TitleLabel.IntrinsicContentSize.Width);

            targetButton.TitleEdgeInsets = titleInsets;
            targetButton.ImageEdgeInsets = imageInsets;
        }

        /// <summary>
        /// Loads an image from a bundle given the supplied image name, resizes it to the
        /// height and width request and sets it into a <see cref="UIButton" />.
        /// </summary>
        /// <param name="source">The <see cref="ImageSource" /> to load the image from.</param>
        /// <param name="widthRequest">The requested image width.</param>
        /// <param name="heightRequest">The requested image height.</param>
        /// <param name="targetButton">A <see cref="UIButton" /> to set the image into.</param>
        /// <param name="state">The state.</param>
        /// <param name="tintColor">Color of the tint.</param>
        /// <returns>A <see cref="Task" /> for the awaited operation.</returns>
        private async static Task SetImageAsync(ImageSource source, double widthRequest, double heightRequest, UIButton targetButton, UIControlState state = UIControlState.Normal, UIColor tintColor = null)
        {
            var handler = GetHandler(source);
            using (UIImage image = await handler.LoadImageAsync(source))
            {
                UIImage scaled = image;
                if (heightRequest > 0 && widthRequest > 0 && (image.Size.Height != heightRequest || image.Size.Width != widthRequest))
                {
                    scaled = scaled.Scale(new CGSize(widthRequest, heightRequest));
                }

                if (tintColor != null)
                {
                    targetButton.TintColor = tintColor;
                    targetButton.SetImage(scaled.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), state);
                }
                else
                    targetButton.SetImage(scaled.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), state);
            }
        }

        /// <summary>
        /// Layouts the subviews.
        /// </summary>
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (ImageButton.Orientation == ImageOrientation.ImageToRight)
            {
                var imageInsets = new UIEdgeInsets(0, Control.Frame.Size.Width - CONTROL_PADDING - (nfloat)ImageButton.ImageWidthRequest, 0, 0);
                Control.ImageEdgeInsets = imageInsets;
            }
        }
    }
}