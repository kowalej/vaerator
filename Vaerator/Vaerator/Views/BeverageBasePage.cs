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
using Vaerator.Helpers;
using System.Diagnostics;
using Vaerator.Ads;
using Vaerator.Services;
using Vaerator.Misc;

namespace Vaerator.Views
{
    public abstract class BeverageBasePage : BasePage
    {
        protected FluidSimulation fluidSim;
        protected View fluidView;
        protected Color fluidColor = new Color(0, 0, 0);
        protected float simResolution = 0.40f;
        protected float viscosityConstant = 0.000000075f;
        protected float diffusionRateConstant = 0.01f;
        protected float gravityConstant = 0.00005000f; // Was 0.00018000f
        protected float terminalVelocityConstant = 0.0052f; // Was 0.052f
        protected BeverageBaseViewModel vm;
        protected List<int> vibrationPattern = new List<int>() { 3000, 600, 4000, 600, 3000, 600, 4000, 600, 3000, 600, 4000, 1000 };
        protected List<MessageType> messagePattern = new List<MessageType>()
        {
            MessageType.FUN,
            MessageType.FACT,
            MessageType.FACT,
        };
        protected MessageSource funMessageSource;
        protected MessageSource factMessageSource;
        protected Object aerateLock = new object();
        protected volatile bool aerating = false;
        protected volatile bool stopping = false;
        protected CancellationTokenSource aerateCancelledSource;
        protected CancellationTokenSource shakeCancelledSource;
        protected ImageButton startAerateButton;
        protected ImageButton stopAerateButton;
        protected Slider durationSlider;
        protected StackLayout durationSliderContainer;
        protected StackLayout glassHereContainer;
        protected Label messageBox;
        protected Stopwatch aerateTimer = new Stopwatch();
        private bool adInitialized = false;

        public BeverageBasePage()
        {

        }

        protected void SetupFluidSim(Grid wineContainer, string staticImageSource)
        {
            // Use dynamic simulation.
            if (Settings.Current.BackgroundSimulationEnabled)
            {
                // Create either SKCanvasView or SKGLView (SKGLView doesn't work on Windows Mobile).
                if (Device.RuntimePlatform == Device.UWP && Device.Idiom == TargetIdiom.Phone)
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
                    Source = Device.RuntimePlatform == Device.UWP ? "Assets/" + staticImageSource : staticImageSource
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
            await StartShake();
            base.OnAppearing();
        }

        protected async override void OnDisappearing()
        {
            await StopAeration();
            if (fluidSim != null) fluidSim.Stop();
            if (shakeCancelledSource != null)
                shakeCancelledSource.Cancel();
            vm.SavePrefs(); // Save preferences here to minimize IO.
            base.OnDisappearing();
        }

        protected async Task StartShake()
        {
            try
            {
                shakeCancelledSource = new CancellationTokenSource();
                Task.Run(async () =>
                {
                    await Task.Delay(500, shakeCancelledSource.Token);
                    ShakeGlass(shakeCancelledSource.Token);
                },
                shakeCancelledSource.Token);
            }
            catch (TaskCanceledException) { }
        }

        private void AnimateOnMain(Action a)
        {
            if (Device.RuntimePlatform != Device.UWP)
            {
                Device.BeginInvokeOnMainThread(a);
            }
            else a.Invoke();
        }

        protected async Task ShakeGlass(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                AnimateOnMain(async () =>
                {
                    var t1 = glassHereContainer.TranslateTo(0, -30, 220);
                    var t2 = glassHereContainer.RotateTo(-3, 220);
                    await Task.WhenAll(t1, t2);

                    await Task.Delay(60);

                    await glassHereContainer.TranslateTo(0, 0, 220);
                    await glassHereContainer.RotateTo(0, 150);
                });

                await Task.Delay(3000, cancellationToken);
            }
        }

        protected async void AerateStart_Clicked(object sender, EventArgs e)
        {
            await StartAeration();
        }

        protected async void AerateStop_Clicked(object sender, EventArgs e)
        {
            await StopAeration();
        }

        protected virtual async Task StartAeration()
        {
            lock (aerateLock)
            {
                if (aerating) return;
                aerating = true; // Start flag.
            }

            CrossKeepAwakeService.Instance.StartAwake(); // Start keep awake.
            aerateTimer.Restart();

            // Log run, stop shaking glass, and fade out.
            if (shakeCancelledSource != null)
                shakeCancelledSource.Cancel();
            AnimateOnMain(async () => await glassHereContainer.FadeTo(0, 200));

            int duration = vm.DurationValue * 1000;
            var tES = EnableStopButton(200);
            var tEM = EnableMessages(200);
            await Task.WhenAll(tES, tEM);
            if (fluidSim != null) fluidSim.LockMotion();
            try
            {
                aerateCancelledSource = new CancellationTokenSource();
                RunVibrationPattern(aerateCancelledSource.Token);
                RunMessagePattern(800, duration, aerateCancelledSource.Token);
                if (!adInitialized)
                {
                    InitializeAd();    
                }
                await Task.Delay(duration, aerateCancelledSource.Token);
                await StopAeration(true);
            }
            catch (TaskCanceledException) { }
            finally { aerateCancelledSource = null; }
        }

        protected virtual async Task StopAeration(bool showFinish = false)
        {
            lock (aerateLock)
            {
                if (!aerating || stopping) return;
                stopping = true;
            }
            CrossKeepAwakeService.Instance.StopAwake(); // Stop keep awake.

            if (aerateCancelledSource != null)
                aerateCancelledSource.Cancel();
            CrossVibrate.Current.StopVibration();
            if (fluidSim != null) fluidSim.UnlockMotion();

            int fadeTimeMillis = 400;

            // Show finish message if we completed full process.
            if (showFinish)
            {
                int totalMessageTime = EstimatedReadTimeMillis(BeverageResources.AerateFinishMessage) + (fadeTimeMillis * 2);
                var tES = EnableStartButton((totalMessageTime + fadeTimeMillis + 200) / 2);
                var tSF = ShowFinishMessage(fadeTimeMillis); 
                var tSDS = Task.Run(async () => { await Task.Delay(totalMessageTime); await EnableDurationSlider(fadeTimeMillis); });
                AnimateOnMain(async () => { await Task.Delay(totalMessageTime); await glassHereContainer.FadeTo(100, (uint)fadeTimeMillis); });
                await Task.WhenAll(tES, tSF, tSDS);
            }
            else
            {
                var tES = EnableStartButton(fadeTimeMillis / 2);
                var tED = EnableDurationSlider(fadeTimeMillis);
                AnimateOnMain(async () => await glassHereContainer.FadeTo(100, (uint)fadeTimeMillis));
                await Task.WhenAll(tES, tED);
            }

            await StartShake(); // Restart shake.
            lock (aerateLock)
            {
                aerating = false; // Stop flag.
                stopping = false;
            }

            // Show ad unless we quickly cancelled (< 5 seconds run time).
            if (aerateTimer.ElapsedMilliseconds >= 5000)
            {
                CrossInterstitialAdService.Instance.ShowAd();
            }
        }

        protected async Task InitializeAd()
        {
            #if DEBUG
                string interstitialAdUnitID = Device.RuntimePlatform == Device.UWP ? UsefulStuff.UWPTest_InterstitialAdUnitID : UsefulStuff.AdMobTest_InterstitialAdUnitID;
            #else
                string interstitialAdUnitID = Device.RuntimePlatform == Device.UWP ? UsefulStuff.UWP_InterstitialAdUnitID : UsefulStuff.AdMob_InterstitialAdUnitID;
            #endif
            CrossInterstitialAdService.Instance.Initialize(interstitialAdUnitID);
            adInitialized = true;
        }

        protected async Task ShowFinishMessage(int fadeTimeMillis)
        {
            AnimateOnMain(async () =>
            {
                if (!messageBox.AnimationIsRunning("FadeTo"))
                    await messageBox.FadeTo(0, (uint)fadeTimeMillis);
                messageBox.Text = BeverageResources.AerateFinishMessage;
                if (!messageBox.AnimationIsRunning("FadeTo"))
                    await messageBox.FadeTo(1.0d, (uint)fadeTimeMillis);
            });
            await Task.Delay(EstimatedReadTimeMillis(BeverageResources.AerateFinishMessage) + (fadeTimeMillis * 2));
        }

        protected async Task RunVibrationPattern(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested) // Keep running pattern until done aerating.
            {
                cancellationToken.ThrowIfCancellationRequested();

                for (int i = 0; i < vibrationPattern.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

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
                    await Task.Delay(vibrationPattern[i], cancellationToken);
                }
            }
        }

        int EstimatedReadTimeMillis(string text, int wordsPerMinute = 250)
        {
            int wordCount = text.Split().Length;
            return (int)(wordCount * (1.0f / wordsPerMinute * 60f) * 1000) + 500;
        }

        protected async Task RunMessagePattern(int fadeTimeMillis, int duration, CancellationToken cancellationToken)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Cancel previous running.
            AnimateOnMain(() =>
            {
                if (messageBox.AnimationIsRunning("FadeTo"))
                    messageBox.AbortAnimation("FadeTo");
            });

            bool initial = true;
            string message = string.Empty;
            int durationRequired = 0;
            while (!cancellationToken.IsCancellationRequested) // Keep running pattern until done aerating.
            {
                cancellationToken.ThrowIfCancellationRequested();

                for (int i = 0; i < messagePattern.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (messagePattern[i] == MessageType.FUN)
                    {
                        message = funMessageSource.GetNext();
                    }
                    else if (messagePattern[i] == MessageType.FACT)
                    {
                        message = factMessageSource.GetNext();
                    }

                    if (initial) durationRequired = EstimatedReadTimeMillis(message) + fadeTimeMillis;
                    else durationRequired = EstimatedReadTimeMillis(message) + (fadeTimeMillis * 2);

                    // Don't show if too close to end, user wont' be able to read text.
                    if (durationRequired <= Math.Abs(duration - sw.ElapsedMilliseconds) + 500)
                    {
                        AnimateOnMain(async () =>
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
                        await Task.Delay(durationRequired, cancellationToken);
                    }
                    else await Task.Delay((int) Math.Abs(duration - sw.ElapsedMilliseconds));
                }
            }
        }

        protected virtual async Task EnableStartButton(int fadeTimeMillis = 80)
        {
            AnimateOnMain(async () =>
            {
                startAerateButton.IsVisible = true;
                await stopAerateButton.FadeTo(0, (uint)fadeTimeMillis);
                await startAerateButton.FadeTo(1.0d, (uint)fadeTimeMillis);
            });
            await Task.Delay(fadeTimeMillis);
            AnimateOnMain(() =>
            {
                stopAerateButton.IsEnabled = false;
                stopAerateButton.IsVisible = false;
                startAerateButton.IsEnabled = true;
            });
        }

        protected virtual async Task EnableStopButton(int fadeTimeMillis = 80)
        {
            AnimateOnMain(async () =>
            {
                stopAerateButton.IsVisible = true;
                await startAerateButton.FadeTo(0, (uint)fadeTimeMillis);
                await stopAerateButton.FadeTo(1.0d, (uint)fadeTimeMillis);
            });
            await Task.Delay(fadeTimeMillis);
            AnimateOnMain(() =>
            {
                startAerateButton.IsEnabled = false;
                startAerateButton.IsVisible = false;
                stopAerateButton.IsEnabled = true;
            });
        }

        protected virtual async Task EnableMessages(int fadeTimeMillis = 80)
        {
            AnimateOnMain(async () =>
            {
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
            AnimateOnMain(async () =>
            {
                messageBox.IsVisible = false;
                durationSliderContainer.IsVisible = true;
                await durationSliderContainer.FadeTo(1.0d, (uint)fadeTimeMillis);
                durationSlider.IsEnabled = true;
            });
            await Task.Delay(fadeTimeMillis);
        }

        protected override abstract void SetTranslationText();
    }
}
