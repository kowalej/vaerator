using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Vaerator.FluidSim
{
    public class SkiaFluidDensityRenderer : IFluidRenderer
    {
        int N, M;
        float resolution;
        float[] d;
        Color baseColor;
        float colorScaleFactor = 1.0f;
        bool canRender = false;
        View view;

        public SkiaFluidDensityRenderer(View view)
        {
            this.view = view;
        }

        public void Setup(float resolution, out int N, out int M)
        {
            float width, height;
            this.resolution = resolution;
            if (view is SKCanvasView)
            {
                width = (view as SKCanvasView).CanvasSize.Width;
                height = (view as SKCanvasView).CanvasSize.Height;
            }
            else
            {
                width = (view as SKGLView).CanvasSize.Width;
                height = (view as SKGLView).CanvasSize.Height;
            }

            float aspectRatio = width / height;
            if (aspectRatio > 1)
            {
                this.N = Math.Max(FluidSimulation.MIN_CELLS, Math.Min(FluidSimulation.MAX_CELLS, (int)Math.Floor(width / (1 / resolution))));
                this.M = (int)Math.Floor(this.N * aspectRatio);
            }
            else
            {
                this.M = Math.Max(FluidSimulation.MIN_CELLS, Math.Min(FluidSimulation.MAX_CELLS, (int)Math.Floor(height / (1 / resolution))));
                this.N = (int)Math.Floor(this.M * aspectRatio);
            }

            // "Sync up" with fluid sim.
            N = this.N;
            M = this.M;

            if (view is SKCanvasView)
            {
                (view as SKCanvasView).PaintSurface += OnCanvasViewPaintSurface;
            }
            else
            {
                (view as SKGLView).PaintSurface += OnGLViewPaintSurface;
                (view as SKGLView).HasRenderLoop = true;
            }
        }

        public void SetDensity(ref float[] d)
        {
            this.d = d;
        }

        public void SetVelocity(ref float[] u, ref float[] v)
        {
            throw new NotImplementedException();
        }

        public void SetColor(Color baseColor, float colorScaleFactor = 1.0f)
        {
            this.baseColor = baseColor;
            this.colorScaleFactor = colorScaleFactor;
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            Draw(args.Surface, args.Info.Width, args.Info.Height);
        }

        void OnGLViewPaintSurface(object sender, SKPaintGLSurfaceEventArgs args)
        {
            Draw(args.Surface, args.RenderTarget.Width, args.RenderTarget.Height);
        }

        void Draw(SKSurface surface, int width, int height)
        {
            Stopwatch sw = new Stopwatch();

            if (canRender)
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);

                sw.Restart();

                using (var paint = new SKPaint())
                {
                    paint.Style = SKPaintStyle.Fill;

                    SKRect rect = new SKRect();
                    Color densityColor;
                    float xCord = 0;
                    float yCord = 0;
                    float xInc, yInc;
                    float colorScale = 1.0f;
                    float density;

                    xInc = width / (float)N;
                    yInc = height / (float)M;

                    // Loop through, match simulation format of first cell (0,0) at bottom left.
                    for (int i = 1; i <= N; i++)
                    {
                        yCord = height;
                        for (int j = 1; j <= M; j++)
                        {
                            // Set bounds of rectangle.
                            rect.Left = xCord;
                            rect.Top = yCord - yInc;
                            rect.Right = xCord + xInc;
                            rect.Bottom = yCord;

                            // Read from corresponding cell to set color based on density. Note bottom left in simulation is index 1,1.
                            density = d[FluidMath.IX(N, i, j)];
                            colorScale = colorScaleFactor * Math.Max(1.0f, Math.Min(1.0f, (1 / density)));
                            if (density > 0.05)
                            {
                                densityColor = new Color(baseColor.R * colorScale, baseColor.G * colorScale, baseColor.B * colorScale, baseColor.A * Math.Pow(density, 1 - Math.Min(1.0, density)));
                                paint.Color = densityColor.ToSKColor();
                                canvas.DrawRect(rect, paint);
                            }

                            // Decrement y for next rectangle.
                            yCord -= yInc;
                        }
                        // Increment x for next rectangle.
                        xCord += xInc;
                    }
                    canvas.Flush();
                }

                var drawTime = sw.ElapsedMilliseconds;
                if (drawTime > 10)
                {
                    #if DEBUG
                        Debug.WriteLine("Slow render time: {0} milliseconds.", drawTime);
                    #endif
                }
            }
        }

        public async Task Render(int timeRemaining)
        {
            canRender = true;
            if (view is SKCanvasView)
            {
                (view as SKCanvasView).InvalidateSurface();
            }
            else
            {
                (view as SKGLView).InvalidateSurface();
            }
            await Task.Delay(timeRemaining);
        }
    }
}
