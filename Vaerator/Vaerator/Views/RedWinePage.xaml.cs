using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Vaerator.FluidSim;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Vaerator.Views
{
    public partial class RedWinePage : BasePage
    {
        FluidSimulation fluidSim;
        View fluidView;

        public RedWinePage()
        {
            InitializeComponent();

            // Create either SKCanvasView or SKGLView (SKGLView doesn't work on Windows Mobile)
            if(Device.RuntimePlatform == "Windows" && Device.Idiom == TargetIdiom.Phone)
            {
                // Setup SKCanvasView
                fluidView = new SKCanvasView();
                (fluidView as SKCanvasView).PaintSurface += FluidCanvasViewPaintSurface; // Temporary event handler, this needs to be called before renderer setup to ensure size is available.
            }
            else
            {
                // Setup SKGLView
                fluidView = new SKGLView();
                (fluidView as SKGLView).PaintSurface += FluidGLViewPaintSurface; // Temporary event handler, this needs to be called before renderer setup to ensure size is available.
            }
            WineContainer.Children.Add(fluidView);
            WineContainer.LowerChild(fluidView);
        }

        private async Task SetupSimulation()
        {
            IFluidRenderer renderer = new SkiaFluidDensityRenderer(fluidView); // Initialize renderer.
            renderer.SetColor(new Color(0.55f, 0, 0)); // Set color to red.
            fluidSim = new FluidSimulation(renderer, 0.40f, 0.0000001f, 0.01f, 0.00018000f, 0.052f, 33); // Initialize simulation.
            await Task.Factory.StartNew(fluidSim.Start, TaskCreationOptions.LongRunning);
        }

        private async void FluidCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            (fluidView as SKCanvasView).PaintSurface -= FluidCanvasViewPaintSurface; // Remove temporary event handler.
            await SetupSimulation();
        }

        private async void FluidGLViewPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            (fluidView as SKGLView).PaintSurface -= FluidGLViewPaintSurface; // Remove temporary event handler.
            await SetupSimulation();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (fluidSim != null && !fluidSim.Running) await Task.Factory.StartNew(fluidSim.Start, TaskCreationOptions.LongRunning);
        }

        protected override void OnDisappearing()
        {
            if (fluidSim != null) fluidSim.Stop();
            base.OnDisappearing();
        }

    }
}
