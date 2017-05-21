using FFImageLoading.Forms;
using SkiaSharp.Views.Forms;
using System.Threading.Tasks;
using Vaerator.FluidSim;
using Xamarin.Forms;
using Vaerator.ViewModels;
using Plugin.Vibrate;
using System;
using System.Collections.Generic;
using System.Threading;
using Vaerator.Controls;
using Localization.Enums;
using Localization.TranslationResources;
using Localization.Localize;
using System.Resources;
using Vaerator.Helpers;
using System.Diagnostics;

namespace Vaerator.Views
{
    public class BeverageBasePage : BasePage
    {
        protected FluidSimulation fluidSim;
        protected View fluidView;
        protected Color fluidColor = new Color(0, 0, 0);
        protected float simResolution = 0.40f;
        protected float viscosityConstant = 0.000000075f;// 0.0000001f;
        protected float diffusionRateConstant = 0.01f;
        protected float gravityConstant = 0.00018000f;
        protected float terminalVelocityConstant = 0.052f;
        protected BeverageBaseViewModel vm;
        protected List<int> vibrationPattern = new List<int>() { 3000, 500, 4000, 500 };
        protected List<Tuple<MessageType, int>> messagePattern = new List<Tuple<MessageType, int>>()
        {
            new Tuple<MessageType, int>(MessageType.FUN, 2000),
            new Tuple<MessageType, int>(MessageType.FACT, 3000),
            new Tuple<MessageType, int>(MessageType.FACT, 3000),
            new Tuple<MessageType, int>(MessageType.FACT, 3000),
        };
        protected MessageSource funMessageSource;
        protected MessageSource factMessageSource;
        protected Object aerateLock = new object();
        protected volatile bool aerating = false;
        protected CancellationTokenSource aerateCancelledSource;
        protected CancellationToken aerateCancelledToken;
        protected ImageButton startAerateButton;
        protected ImageButton stopAerateButton;
        protected Slider durationSlider;
        protected StackLayout durationSliderContainer;
        protected Label messageBox;

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

        protected async override void OnDisappearing()
        {
            await StopAeration();
            if (fluidSim != null) fluidSim.Stop();
            base.OnDisappearing();
        }

        protected async void AerateStart_Clicked(object sender, EventArgs e)
        {
            (sender as ImageButton).IsEnabled = false; // Very important, prevents double clicks!
            await StartAeration();
        }

        protected async void AerateStop_Clicked(object sender, EventArgs e)
        {
            (sender as ImageButton).IsEnabled = false; // Very important, prevents double clicks!
            await StopAeration();
        }

        protected virtual async Task StartAeration()
        {
            if (!aerating)
            {
                lock (aerateLock)
                {
                    aerating = true; // Start flag.
                }
                try
                {
                    int duration = vm.DurationValue * 1000;
                    var tES = EnableStopButton(200);
                    var tEM = EnableMessages(200);
                    await Task.WhenAll(tES, tEM);
                    aerateCancelledSource = new CancellationTokenSource();
                    aerateCancelledToken = aerateCancelledSource.Token;
                    if (fluidSim != null) fluidSim.LockMotion();
                    aerateCancelledToken = aerateCancelledSource.Token;
                    Task.Run(() => RunVibrationPattern(aerateCancelledToken));
                    Task.Run(() => RunMessagePattern(800, duration, aerateCancelledToken));
                    await Task.Delay(duration, aerateCancelledToken);
                    await StopAeration();
                }
                catch(TaskCanceledException)
                {
                    return;
                }
                finally
                {
                    aerateCancelledSource = null;
                }
            }
        }
        
        protected virtual async Task StopAeration()
        {
            if (aerateCancelledSource != null)
                aerateCancelledSource.Cancel();
            CrossVibrate.Current.StopVibration();
            if (fluidSim != null) fluidSim.UnlockMotion();
            var tES = EnableStartButton(200);
            var tED = EnableDurationSlider(200);
            await Task.WhenAll(tES, tED);
            lock (aerateLock)
            {
                aerating = false; // Stop flag.
            }
        }

        protected async Task RunVibrationPattern(CancellationToken aerateCancelledToken)
        {
            while (true) // Keep running pattern until done aerating.
            {
                if (aerateCancelledToken.IsCancellationRequested)
                {
                    return;
                }

                for (int i = 0; i < vibrationPattern.Count; i++)
                {
                    if (aerateCancelledToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (i % 2 == 0)
                    {
                        if (fluidSim != null) fluidSim.Vibrating = true;
                        CrossVibrate.Current.StartVibration(vibrationPattern[i]);
                    }
                    else
                    {
                        if (fluidSim != null) fluidSim.Vibrating = false;
                        CrossVibrate.Current.StopVibration();
                    }
                    await Task.Delay(vibrationPattern[i], aerateCancelledToken);
                }
            }
        }

        protected async Task RunMessagePattern(int fadeTimeMillis, int duration, CancellationToken aerateCancelledToken)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Cancel previous running.
            Device.BeginInvokeOnMainThread(() =>
            {
                if(messageBox.AnimationIsRunning("FadeTo"))
                    messageBox.AbortAnimation("FadeTo");
            });

            bool initial = true;
            string message = string.Empty;
            while (true) // Keep running pattern until done aerating.
            {
                if (aerateCancelledToken.IsCancellationRequested)
                {
                    return;
                }

                for (int i = 0; i < messagePattern.Count; i++)
                {
                    if (aerateCancelledToken.IsCancellationRequested)
                    {
                        return;
                    }

                    int durationRequired = messagePattern[i].Item2 + (fadeTimeMillis * 2);
                    // Don't show if too close to end.
                    if (durationRequired <= Math.Abs(duration - sw.ElapsedMilliseconds) + 500)
                    {
                        if (messagePattern[i].Item1 == MessageType.FUN)
                        {
                            message = funMessageSource.GetNext();
                        }
                        else if (messagePattern[i].Item1 == MessageType.FACT)
                        {
                            message = factMessageSource.GetNext();
                        }
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            if (!initial) // Don't fade out on first.
                        {
                                if (!messageBox.AnimationIsRunning("FadeTo"))
                                    await messageBox.FadeTo(0, (uint)fadeTimeMillis);
                            }
                            initial = false; // Always fade from now on.
                        messageBox.Text = message;
                            if (!messageBox.AnimationIsRunning("FadeTo"))
                                await messageBox.FadeTo(1.0d, (uint)fadeTimeMillis);
                        });
                        await Task.Delay(durationRequired, aerateCancelledToken);
                    }
                }
            }
        }

        protected virtual async Task EnableStartButton(int fadeTimeMillis = 80)
        {
            Device.BeginInvokeOnMainThread(async () => {
                startAerateButton.IsVisible = true;
                await stopAerateButton.FadeTo(0, (uint)fadeTimeMillis);
                await startAerateButton.FadeTo(1.0d, (uint)fadeTimeMillis);
            });
            await Task.Delay(fadeTimeMillis);
            Device.BeginInvokeOnMainThread(() =>
            {
                stopAerateButton.IsEnabled = false;
                stopAerateButton.IsVisible = false;
                startAerateButton.IsEnabled = true;
            });
        }

        protected virtual async Task EnableStopButton(int fadeTimeMillis = 80)
        {
            Device.BeginInvokeOnMainThread(async () => {
                stopAerateButton.IsVisible = true;
                await startAerateButton.FadeTo(0, (uint)fadeTimeMillis);
                await stopAerateButton.FadeTo(1.0d, (uint)fadeTimeMillis);
            });
            await Task.Delay(fadeTimeMillis);
            Device.BeginInvokeOnMainThread(() =>
            {
                startAerateButton.IsEnabled = false;
                startAerateButton.IsVisible = false;
                stopAerateButton.IsEnabled = true;
            });
        }

        protected virtual async Task EnableMessages(int fadeTimeMillis = 80)
        {
            Device.BeginInvokeOnMainThread(async () => {
                messageBox.IsVisible = true;
                messageBox.Opacity = 0; // Prep for initial fade in.
                await durationSliderContainer.FadeTo(0, (uint)fadeTimeMillis);
                durationSliderContainer.IsVisible = false;
                durationSlider.IsEnabled = false;
            });
            await Task.Delay(fadeTimeMillis);
        }

        protected virtual async Task EnableDurationSlider(int fadeTimeMillis = 80)
        {
            Device.BeginInvokeOnMainThread(async () => {
                messageBox.IsVisible = false;
                durationSliderContainer.IsVisible = true;
                await durationSliderContainer.FadeTo(1.0d, (uint)fadeTimeMillis);
                durationSlider.IsEnabled = true;
            });
            await Task.Delay(fadeTimeMillis);
        }
    }
}
