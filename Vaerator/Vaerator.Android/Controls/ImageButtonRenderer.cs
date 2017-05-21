using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Vaerator.Enums;
using Color = Xamarin.Forms.Color;
using Vaerator.Controls;
using Vaerator.Droid.Helpers;

[assembly: ExportRenderer(typeof(ImageButton), typeof(ImageButtonRenderer))]
namespace Vaerator.Controls
{
    /// <summary>
    /// Draws a button on the Android platform with the image shown in the right 
    /// position with the right size.
    /// </summary>
    public partial class ImageButtonRenderer : Xamarin.Forms.Platform.Android.ButtonRenderer
    {
        /// <summary>
        /// Gets the underlying control typed as an <see cref="ImageButton"/>.
        /// </summary>
        private ImageButton ImageButton
        {
            get { return (ImageButton)Element; }
        }

        /// <summary>
        /// Sets up the button including the image. 
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected async override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            var targetButton = Control;

            if (Element != null && Element.Font != Font.Default && targetButton != null) targetButton.Typeface = Element.Font.ToExtendedTypeface(Context);

            if (Element != null && ImageButton.Source != null) await SetImageSourceAsync(targetButton, ImageButton).ConfigureAwait(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && Control != null)
            {
                Control.Dispose();
            }
        }

        public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            return base.GetDesiredSize(widthConstraint, heightConstraint);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            if (Element != null && (ImageButton.Orientation == ImageOrientation.ImageCenterToLeft || ImageButton.Orientation == ImageOrientation.ImageCenterToRight))
            {
                Rect drawableBounds = new Rect();
                Rect textBounds = new Rect();

                var drawables = Control.GetCompoundDrawables();
                Control.Paint.GetTextBounds(Control.Text, 0, Control.Text.Length, textBounds);

                if (drawables[0] != null)
                {
                    drawables[0].CopyBounds(drawableBounds);
                    int totalShift = (Control.Width / 2) - ((drawableBounds.Width() + textBounds.Width()) / 2) - (Control.CompoundDrawablePadding / 2);
                    Control.SetPadding(totalShift, Control.PaddingTop, Control.PaddingRight, Control.PaddingBottom);
                }

                //Right
                else if (drawables[2] != null)
                {
                    drawables[2].CopyBounds(drawableBounds);
                    int totalShift = (Control.Width / 2) - ((drawableBounds.Width() + textBounds.Width()) / 2) - (Control.CompoundDrawablePadding / 2);
                    Control.SetPadding(Control.PaddingLeft, Control.PaddingTop, totalShift, Control.PaddingBottom);
                }
            }

            // Ensures overly wide button images have min padding.
            else if (Element != null && (ImageButton.Orientation == ImageOrientation.ImageOnTop || ImageButton.Orientation == ImageOrientation.ImageOnBottom))
            {
                var drawables = Control.GetCompoundDrawables();
                Drawable image;
                if (drawables[0] != null)
                    image = drawables[0];
                else if (drawables[1] != null)
                    image = drawables[1];
                else if (drawables[2] != null)
                    image = drawables[2];
                else if (drawables[3] != null)
                    image = drawables[3];
                else return;

                var widthd = image.Bounds.Width();
                var thresh = Forms.Context.ToPixels(MIN_IMAGE_PADDING * 2);
                if (widthd > Control.Width - thresh)
                {
                    var diff = (widthd - (Control.Width - thresh)) / 2;
                    image.Bounds.Set(image.Bounds.Left + (int)diff, image.Bounds.Top, image.Bounds.Right - (int)diff, image.Bounds.Bottom);
                }
            }
            base.OnLayout(changed, l, t, r, b);

        }

        /// <summary>
        /// Sets the image source.
        /// </summary>
        /// <param name="targetButton">The target button.</param>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task"/> for the awaited operation.</returns>
        private async Task SetImageSourceAsync(Android.Widget.Button targetButton, ImageButton model)
        {
            if (targetButton == null || targetButton.Handle == IntPtr.Zero || model == null) return;

            var source = model.IsEnabled ? model.Source : model.DisabledSource ?? model.Source;

            using (var bitmap = await GetBitmapAsync(source).ConfigureAwait(false))
            {
                if (bitmap == null)
                    targetButton.SetCompoundDrawables(null, null, null, null);
                else
                {
                    var drawable = new BitmapDrawable(bitmap);
                    var tintColor = model.IsEnabled ? model.ImageTintColor : model.DisabledImageTintColor;
                    if (tintColor != Color.Transparent)
                    {
                        drawable.SetTint(tintColor.ToAndroid());
                        drawable.SetTintMode(PorterDuff.Mode.SrcIn);
                    }

                    using (var scaledDrawable = GetScaleDrawable(drawable, model.ImageWidthRequest, model.ImageHeightRequest))
                    {
                        Drawable left = null;
                        Drawable right = null;
                        Drawable top = null;
                        Drawable bottom = null;
                        targetButton.CompoundDrawablePadding = (int)Context.ToPixels(10);

                        switch (model.Orientation)
                        {
                            case ImageOrientation.ImageToLeft:
                                targetButton.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
                                left = scaledDrawable;
                                break;
                            case ImageOrientation.ImageToRight:
                                targetButton.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                                right = scaledDrawable;
                                break;
                            case ImageOrientation.ImageOnTop:
                                targetButton.Gravity = GravityFlags.Top | GravityFlags.CenterHorizontal;
                                top = scaledDrawable;
                                break;
                            case ImageOrientation.ImageOnBottom:
                                targetButton.Gravity = GravityFlags.Bottom | GravityFlags.CenterHorizontal;
                                bottom = scaledDrawable;
                                break;
                            case ImageOrientation.ImageCenterToLeft:
                                targetButton.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
                                left = scaledDrawable;
                                break;
                            case ImageOrientation.ImageCenterToRight:
                                targetButton.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                                right = scaledDrawable;
                                break;
                        }
                        targetButton.SetMinimumWidth(scaledDrawable.Bounds.Width() + Control.PaddingLeft + Control.PaddingRight);
                        targetButton.SetMinimumHeight(scaledDrawable.Bounds.Height() + Control.PaddingTop + Control.PaddingBottom);
                        targetButton.SetCompoundDrawables(left, top, right, bottom);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="Bitmap"/> for the supplied <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="source">The <see cref="ImageSource"/> to get the image for.</param>
        /// <returns>A loaded <see cref="Bitmap"/>.</returns>
        private async Task<Bitmap> GetBitmapAsync(ImageSource source)
        {
            var handler = GetHandler(source);
            var returnValue = (Bitmap)null;

            if (handler != null)
                returnValue = await handler.LoadImageAsync(source, Context).ConfigureAwait(false);

            return returnValue;
        }

        /// <summary>
        /// Called when the underlying model's properties are changed.
        /// </summary>
        /// <param name="sender">The Model used.</param>
        /// <param name="e">The event arguments.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ImageButton.SourceProperty.PropertyName ||
                e.PropertyName == ImageButton.DisabledSourceProperty.PropertyName ||
                e.PropertyName == VisualElement.IsEnabledProperty.PropertyName ||
                e.PropertyName == ImageButton.ImageTintColorProperty.PropertyName ||
                e.PropertyName == ImageButton.DisabledImageTintColorProperty.PropertyName)
            {
                // CURRENTLY BROKEN
                // await SetImageSourceAsync(Control, ImageButton).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Returns a <see cref="Drawable"/> with the correct dimensions from an 
        /// Android resource id.
        /// </summary>
        /// <param name="drawable">An android <see cref="Drawable"/>.</param>
        /// <param name="width">The width to scale to.</param>
        /// <param name="height">The height to scale to.</param>
        /// <returns>A scaled <see cref="Drawable"/>.</returns>
        private Drawable GetScaleDrawable(Drawable drawable, double width, double height)
        {
            var returnValue = new ScaleDrawable(drawable, GravityFlags.Center, 100, 100).Drawable;
            returnValue.SetBounds(0, 0, (int)Context.ToPixels(width), (int)Context.ToPixels(height));
            return returnValue;
        }
    }
}