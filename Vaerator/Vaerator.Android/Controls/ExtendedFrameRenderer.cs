using System.ComponentModel;
using Android.Graphics;
using Android.Graphics.Drawables;
using ACanvas = Android.Graphics.Canvas;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Vaerator.Controls;
using Vaerator.Droid.Controls;

[assembly: ExportRenderer(typeof(ExtendedFrame), typeof(ExtendedFrameRenderer))]
namespace Vaerator.Droid.Controls
{
    public class ExtendedFrameRenderer : VisualElementRenderer<ExtendedFrame>
    {
        bool _disposed;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && !_disposed)
            {
                Background.Dispose();
                _disposed = true;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ExtendedFrame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && e.OldElement == null)
            {
                UpdateBackground();
                UpdateCornerRadius();
            }
        }

        void UpdateBackground()
        {
            this.SetBackground(new FrameDrawable(Element));
        }

        void UpdateCornerRadius()
        {
            this.SetBackground(new FrameDrawable(Element));
        }

        class FrameDrawable : Drawable
        {
            readonly ExtendedFrame _frame;

            bool _isDisposed;
            Bitmap _normalBitmap;

            public FrameDrawable(ExtendedFrame frame)
            {
                _frame = frame;
                frame.PropertyChanged += FrameOnPropertyChanged;
            }

            public override bool IsStateful
            {
                get { return false; }
            }

            public override int Opacity
            {
                get { return 0; }
            }

            public override void Draw(ACanvas canvas)
            {
                int width = Bounds.Width();
                int height = Bounds.Height();

                if (width <= 0 || height <= 0)
                {
                    if (_normalBitmap != null)
                    {
                        _normalBitmap.Dispose();
                        _normalBitmap = null;
                    }
                    return;
                }

                if (_normalBitmap == null || _normalBitmap.Height != height || _normalBitmap.Width != width)
                {
                    // If the user changes the orientation of the screen, make sure to destroy reference before
                    // reassigning a new bitmap reference.
                    if (_normalBitmap != null)
                    {
                        _normalBitmap.Dispose();
                        _normalBitmap = null;
                    }

                    _normalBitmap = CreateBitmap(false, width, height);
                }
                Bitmap bitmap = _normalBitmap;
                using (var paint = new Paint())
                    canvas.DrawBitmap(bitmap, 0, 0, paint);
            }

            public override void SetAlpha(int alpha)
            {
            }

            public override void SetColorFilter(ColorFilter cf)
            {
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing && !_isDisposed)
                {
                    if (_normalBitmap != null)
                    {
                        _normalBitmap.Dispose();
                        _normalBitmap = null;
                    }

                    _isDisposed = true;
                }

                base.Dispose(disposing);
            }

            protected override bool OnStateChange(int[] state)
            {
                return false;
            }

            Bitmap CreateBitmap(bool pressed, int width, int height)
            {
                Bitmap bitmap;
                using (Bitmap.Config config = Bitmap.Config.Argb8888)
                    bitmap = Bitmap.CreateBitmap(width, height, config);

                using (var canvas = new ACanvas(bitmap))
                {
                    DrawCanvas(canvas, width, height, pressed);
                }

                return bitmap;
            }

            void DrawBackground(ACanvas canvas, int width, int height, float cornerRadius, float outlineWidth, bool pressed)
            {
                using (var paint = new Paint { AntiAlias = true })
                using (var rect = new RectF(outlineWidth, outlineWidth, width - outlineWidth, height - outlineWidth))
                {
                    paint.SetStyle(Paint.Style.Fill);
                    paint.Color = _frame.BackgroundColor.ToAndroid();
                    canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                }
            }

            void DrawOutline(ACanvas canvas, int width, int height, float cornerRadius, float outlineWidth)
            {
                using (var paint = new Paint { AntiAlias = true })
                using (var rect = new RectF(0, 0, width, height))
                {
                    paint.StrokeWidth = outlineWidth;
                    paint.SetStyle(Paint.Style.Fill);
                    paint.Color = _frame.OutlineColor.ToAndroid();
                    canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);

                    // This will "carve out" the center where our background layer will go. Therefore we maintain the proper background color/transparency.
                    using (var clearXfer = new PorterDuffXfermode(PorterDuff.Mode.Clear))
                    {
                        paint.SetXfermode(clearXfer);
                        rect.Left = outlineWidth; rect.Top = outlineWidth; rect.Right = width - outlineWidth; rect.Bottom = height - outlineWidth;
                        canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                    }
                }
            }

            void FrameOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
                    e.PropertyName == ExtendedFrame.OutlineColorProperty.PropertyName ||
                    e.PropertyName == ExtendedFrame.CornerRadiusProperty.PropertyName ||
                    e.PropertyName == ExtendedFrame.OutlineWidthProperty.PropertyName)
                {
                    if (_normalBitmap == null)
                        return;

                    using (var canvas = new ACanvas(_normalBitmap))
                    {
                        int width = Bounds.Width();
                        int height = Bounds.Height();
                        canvas.DrawColor(global::Android.Graphics.Color.Black, PorterDuff.Mode.Clear);
                        DrawCanvas(canvas, width, height, false);
                    }
                    InvalidateSelf();
                }
            }

            void DrawCanvas(ACanvas canvas, int width, int height, bool pressed)
            {
                float cornerRadius = _frame.CornerRadius;
                if (cornerRadius == -1f)
                    cornerRadius = 5f; // Default corner radius - converted
                else cornerRadius = Forms.Context.ToPixels(cornerRadius);

                float outlineWidth = 0;
                if(_frame.OutlineWidth > 0)
                {
                    outlineWidth = Forms.Context.ToPixels(_frame.OutlineWidth);
                    DrawOutline(canvas, width, height, cornerRadius, outlineWidth);
                }
                DrawBackground(canvas, width, height, cornerRadius, outlineWidth, pressed);
            }
        }
    }
}
