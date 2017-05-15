using FFImageLoading.Forms;
using SkiaSharp.Views.Forms;
using System.Threading.Tasks;
using Vaerator.FluidSim;
using Xamarin.Forms;
using Vaerator.ViewModels;

namespace Vaerator.Views
{
    public class BeverageBasePage : BasePage
    {
        protected FluidSimulation fluidSim;
        protected View fluidView;
        protected Color fluidColor = new Color(0, 0, 0);
        protected float simResolution = 0.40f;
        protected float viscosityConstant = 0.0000001f;
        protected float diffusionRateConstant = 0.01f;
        protected float gravityConstant = 0.00018000f;
        protected float terminalVelocityConstant = 0.052f;

        protected void SetupFluidSim(Grid wineContainer, string staticImageSource)
        {
            // Use dynamic simulation.
            if (Settings.Current.BackgroundSimulationEnabled)
            {
                // Create either SKCanvasView or SKGLView (SKGLView doesn't work on Windows Mobile).
                if (Device.RuntimePlatform == "Windows" && Device.Idiom == TargetIdiom.Phone)
                {
                    // Setup SKCanvasView.
                    fluidView = new SKCanvasView();
                    (fluidView as SKCanvasView).PaintSurface += FluidCanvasViewPaintSurface; // Temporary event handler, this needs to be called before renderer setup to ensure size is available.
                }
                else
                {
                    // Setup SKGLView.
                    fluidView = new SKGLView();
                    (fluidView as SKGLView).PaintSurface += FluidGLViewPaintSurface; // Temporary event handler, this needs to be called before renderer setup to ensure size is available.
                }
            }

            // Use static background.
            else
            {
                fluidView = new CachedImage()
                {
                    Aspect = Aspect.Fill,
                    DownsampleToViewSize = true,
                    Source = Device.RuntimePlatform == Device.Windows ? "Assets/" + staticImageSource : staticImageSource
                };
            }

            wineContainer.Children.Add(fluidView);
            wineContainer.LowerChild(fluidView);
        }

        protected async Task SetupSimulation()
        {
            IFluidRenderer renderer = new SkiaFluidDensityRenderer(fluidView); // Initialize renderer.
            renderer.SetColor(fluidColor); // Set color to red.
           
            fluidSim = new FluidSimulation(renderer, simResolution, viscosityConstant, diffusionRateConstant, gravityConstant, terminalVelocityConstant, 33); // Initialize simulation. //  0.40f, 0.0000001f, 0.01f, 0.00018000f, 0.052f
            await Task.Factory.StartNew(fluidSim.Start, TaskCreationOptions.LongRunning);
        }

        protected async void FluidCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            (fluidView as SKCanvasView).PaintSurface -= FluidCanvasViewPaintSurface; // Remove temporary event handler.
            await SetupSimulation();
        }

        protected async void FluidGLViewPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            (fluidView as SKGLView).PaintSurface -= FluidGLViewPaintSurface; // Remove temporary event handler.
            await SetupSimulation();
        }

        protected override async void OnAppearing()
        {
            if (fluidSim != null && !fluidSim.Running) await Task.Factory.StartNew(fluidSim.Start, TaskCreationOptions.LongRunning);
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            if (fluidSim != null) fluidSim.Stop();
            base.OnDisappearing();
        }
    }
}
