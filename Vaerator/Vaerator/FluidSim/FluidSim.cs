using System;
using System.Diagnostics;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using Vaerator.Services;
using Xamarin.Forms;

namespace Vaerator.FluidSim
{
    public class FluidSimulation
    {
        public const int MAX_CELLS = 120; // In either direction.
        public const int MIN_CELLS = 20;
        public const float FORCE_COVERAGE = 0.12f; 
        public const float SPIN_FORCE_COVERAGE = 0.10f;
        public const int SPIN_TIME = 1800;

        IFluidRenderer renderer;
        int N; // Number of horizontal cells.
        int M; // Number of vertical cells.
        float[] u, uS; // Horizontal velocity and velocity source components.
        float[] v, vS; // Vertical velocity and velocity source components.
        float[] d, dS; // Densisty and density source values.
        float dt; // How fast to run simulation (milliseconds between simulation steps).
        float resolution; // Resolution of renderering, from 0.0 to 1.0. The higher the resolution, the nice the simulation looks.
        volatile bool running;// Flag's whether or not the simulation is running.
        public bool Running { get { return running; } }
        int size; // Size of velocity/density arrays.
        readonly float GRAVITY_CONSTANT, TERMINAL_VELOCITY_CONSTANT, VISCOSITY_CONSTANT, DIFFUSION_RATE_CONSTANT; // Model constants.
        float viscosity, diffusion, gravity, terminalVelocity; // Computed values.
        volatile MotionVector deviceMotion = new MotionVector() { X = 1, Y = 1, Z = 1 };
        volatile bool motionLocked = false;
        int spin = -1; // Spin direction.
        Stopwatch lastSpin = new Stopwatch(); // Timer between spin direction change.
        Stopwatch lastSpinMove = new Stopwatch(); // Timer between spin direction change.
        Random spinRandom = new Random(); // Randomizer for spin location.
        bool vibrating = false;
        public bool Vibrating { get { return vibrating; } set { vibrating = value; } }
        int spinOffsetAddX = 0, spinOffsetAddY = 0;
        CancellationTokenSource simCancelledSource;

        /// <summary>
        /// Create fluid simulation.
        /// </summary>
        /// <param name="renderer">Renderer to use for drawing.</param>
        /// <param name="resolution">Resolution of renderering, from 0.0 to 1.0. The higher the resolution, the nice the simulation looks.</param>
        /// <param name="viscosity">Viscosity of the fluid.</param>
        /// <param name="diffusionRate">Diffusion rate of the fluid.</param>
        /// <param name="deltaTime">How fast to run simulation (milliseconds between simulation steps).</param>
        public FluidSimulation(IFluidRenderer renderer, float resolution, float viscosityConstant, float diffusionRateConstant, float gravityConstant, float terminalVelocityConstant, float deltaTime)
        {
            this.renderer = renderer;
            this.resolution = resolution;
            VISCOSITY_CONSTANT = viscosityConstant;
            DIFFUSION_RATE_CONSTANT = diffusionRateConstant;
            GRAVITY_CONSTANT = gravityConstant;
            TERMINAL_VELOCITY_CONSTANT = terminalVelocityConstant;
            dt = deltaTime;
            deviceMotion = new MotionVector();
            deviceMotion.X = 0.0d; deviceMotion.Y = 1.0d; deviceMotion.Z = 9.8d;
            SetupSimulation();
        }

        void SetupSimulation()
        {
            // Setup the renderer and determine number of horizontal and vertical cells. Will cap at max N / M inputs.
            renderer.Setup(resolution, out N, out M);

            size = (N + 2) * (M + 2);

            u = new float[size]; v = new float[size];
            uS = new float[size]; vS = new float[size];
            d = new float[size]; dS = new float[size];
            ResetSimulation();
        }

        void ResetSimulation()
        {
            // Zero out the arrays.
            for (int i = 0; i < size; i++)
            {
                u[i] = 0; v[i] = 0;
                uS[i] = 0; vS[i] = 0;
                d[i] = 0; dS[i] = 0;
            }

            // Initialize model parameters.
            viscosity = VISCOSITY_CONSTANT;
            gravity = GRAVITY_CONSTANT / (1000 / dt) * (N * M / 2);
            terminalVelocity = TERMINAL_VELOCITY_CONSTANT / (1000 / dt) * (N * M / 2);

            // Add initial density source.
            AddCenteredRect(0.5f, 1.0f);

            // Diffuse a little for rounded effect.
            diffusion = 0.012f;
            for (int i = 0; i < 2; i++)
            {
                SimulateStep();
            }

            // Normal diffusion rate.
            diffusion = 0.0f; //DIFFUSION_RATE_CONSTANT;

            // Pass density to renderer so it knows what to draw.
            renderer.SetDensity(ref d);
        }

        int GetXH(float coverage)
        {
            if (N % 2 == 0)
                return Helpers.Misc.RoundToEven(N * coverage); // Round to even.
            else return Helpers.Misc.RoundToOdd(N * coverage); // Round to odd.
        }

        int GetYH(float coverage)
        {
            if (M % 2 == 0)
                return Helpers.Misc.RoundToEven(M * coverage); // Round to even.
            else return Helpers.Misc.RoundToOdd(M * coverage); // Round to odd.
        }

        int GetOffsetX(int xH)
        {
            return 1 + ((N - xH) / 2);
        }

        int GetOffsetY(int yH)
        {
            return 1 + ((M - yH) / 2);
        }

        void AddCenteredRect(float coverage, float densityFull)
        {
            int xH = GetXH(coverage);
            int yH = GetYH(coverage);
            int offsetX = GetOffsetX(xH);
            int offsetY = GetOffsetY(yH);

            float fill = ((N * M) / (xH * yH)) * densityFull;

            for (int i = offsetX; i <= offsetX + xH - 1; i++)
            {
                for (int j = offsetY; j <= offsetY + yH - 1; j++)
                {
                    dS[FluidMath.IX(N, i, j)] = fill;
                }
            }
        }

        public async Task Start()
        {
            Stopwatch stopWatchFrame = new Stopwatch();
            running = true;
            simCancelledSource = new CancellationTokenSource();

            if (Device.RuntimePlatform == Device.iOS)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    // Main thread or else queue can be null...
                    StartMotionCapture();
                });
            }
            else StartMotionCapture();

            while (!simCancelledSource.IsCancellationRequested)
            {
                if (stopWatchFrame.ElapsedMilliseconds >= dt || !stopWatchFrame.IsRunning)
                {
                    stopWatchFrame.Restart();

                    // Run simulation step.
                    SimulateStep();

                    if (!motionLocked)
                    {
                        ClearSpin();
                        AddForces();
                    }
                    else if (motionLocked && vibrating)
                    {
                        Spin();
                    }
                    else ClearSpin();

                    if (stopWatchFrame.ElapsedMilliseconds > dt)
                    {
                        var elapsed = stopWatchFrame.ElapsedMilliseconds;
                        #if DEBUG
                            Debug.WriteLine("Slow frame time: {0} milliseconds.", elapsed);
                        #endif
                    }
                    await renderer.Render((int)Math.Max(1, dt - stopWatchFrame.ElapsedMilliseconds));
                }
            }
        }

        public void Stop()
        {
            if(simCancelledSource != null)
                simCancelledSource.Cancel();
            running = false;
            StopMotionCapture();
        }

        public void LockMotion() { motionLocked = true; }

        public void UnlockMotion() { motionLocked = false; }

        void StartMotionCapture()
        {
            CrossDeviceMotion.Current.SensorValueChanged += AccelSensorValueChanged;
            CrossDeviceMotion.Current.Start(MotionSensorType.Accelerometer, MotionSensorDelay.Ui);
        }

        void StopMotionCapture()
        {
            CrossDeviceMotion.Current.Stop(MotionSensorType.Accelerometer);
            CrossDeviceMotion.Current.SensorValueChanged -= AccelSensorValueChanged;
        }

        public static MotionVector FixAccelerometerRelative(MotionVector deviceMotion)
        {
            MotionVector result = deviceMotion;

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                var rotation = CrossRotationService.Instance.GetRotation();
                int[][] axisSwaps = new int[4][]
                {
                        new int[4]{  1, -1,  0,  1  },  // ROTATION_0 
                        new int[4]{ -1, -1,  1,  0  },   // ROTATION_90 
                        new int[4]{ -1,  1,  0,  1  },  // ROTATION_180 
                        new int[4]{  1,  1,  1,  0  }  // ROTATION_270 
                };

                double[] motionVals = new double[3] { deviceMotion.X, deviceMotion.Y, deviceMotion.Z };
                int[] axisSwap = axisSwaps[(int)rotation];
                result.X = (double)axisSwap[0] * motionVals[axisSwap[2]];
                result.Y = (double)axisSwap[1] * motionVals[axisSwap[3]] * -1; // Need to flip Y.
                result.Z = motionVals[2];
            }

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                result.X *= -1;
                result.Y *= -1; 
            }
            return result;
        }

        void AccelSensorValueChanged(object sender, SensorValueChangedEventArgs e)
        {
            if (e.SensorType == MotionSensorType.Accelerometer)
            {
                deviceMotion = FixAccelerometerRelative((MotionVector)e.Value);

                #if DEBUG
                    Debug.WriteLine("X: {0}, Y: {1}, Z{2}", deviceMotion.X, deviceMotion.Y, deviceMotion.Z);
                #endif
            }
        }

        void AddForces()
        {
            float cellVelocity = 0, addU = 0, addV = 0;
            int xH = GetXH(FORCE_COVERAGE); int yH = GetYH(FORCE_COVERAGE);
            int average = (xH + yH) / 2;
            xH = average; yH = average;
            int offsetX = GetOffsetX(xH);
            int offsetY = GetOffsetY(yH);

            // Normalize and reverse X & Y direction.
            MotionVector normalizedMotion = new MotionVector();
            double normal = 1.0d; //; Math.Sqrt(Math.Pow(deviceMotion.X, 2) + Math.Pow(deviceMotion.Y, 2) + Math.Pow(deviceMotion.Z, 2));
            normalizedMotion.X = deviceMotion.X / -normal; normalizedMotion.Y = deviceMotion.Y / -normal;
            
            for (int i = offsetX; i <= offsetX + xH - 1; i++)
            {
                for (int j = offsetY; j <= offsetY + yH - 1; j++)
                {
                    if (d[FluidMath.IX(N, i, j)] > 0)
                    {
                        addU = gravity * (float)normalizedMotion.X * d[FluidMath.IX(N, i, j)];
                        addV = gravity * (float)normalizedMotion.Y * d[FluidMath.IX(N, i, j)];
                    }
                    else
                    {
                        addU = gravity * (float)normalizedMotion.X;
                        addV = gravity * (float)normalizedMotion.Y;
                    }
                    uS[FluidMath.IX(N, i, j)] = addU;
                    vS[FluidMath.IX(N, i, j)] = addV;

                    // Set u.
                    cellVelocity = u[FluidMath.IX(N, i, j)] + addU;
                    if (cellVelocity < -terminalVelocity)
                    {
                        uS[FluidMath.IX(N, i, j)] = addU - (cellVelocity - -terminalVelocity);
                    }
                    else if (cellVelocity > terminalVelocity)
                    {
                        uS[FluidMath.IX(N, i, j)] = addU - (cellVelocity - terminalVelocity);
                    }
                    else uS[FluidMath.IX(N, i, j)] = addU;

                    // Set v.
                    cellVelocity = v[FluidMath.IX(N, i, j)] + addV;
                    if (cellVelocity < -terminalVelocity)
                    {
                        vS[FluidMath.IX(N, i, j)] = addV - (cellVelocity - -terminalVelocity);;
                    }
                    else if (cellVelocity > terminalVelocity)
                    {
                        vS[FluidMath.IX(N, i, j)] = addV - (cellVelocity - terminalVelocity);
                    }
                    else vS[FluidMath.IX(N, i, j)] = addV;
                }
            }
        }
        
        void ClearSpin()
        {
            // Rest offset modifiers.
            spinOffsetAddX = 0;
            spinOffsetAddY = 0;

            // Stop spin timers.
            lastSpin.Stop();
            lastSpinMove.Stop();
        }

        void Spin()
        {
            float cellVelocity = 0, addU = 0, addV = 0;
            int xH = GetXH(SPIN_FORCE_COVERAGE); int yH = GetYH(SPIN_FORCE_COVERAGE);
            int average = (xH + yH) / 2;
            xH = average; yH = average;

            int offsetX = GetOffsetX(xH);
            int offsetY = GetOffsetY(yH);

            float gravity = this.gravity * 1.2f; // Extra force for spin.

            // Normalize and reverse X & Y direction.
            MotionVector normalizedMotion = new MotionVector();

            normalizedMotion.X = (double)N / (N + M) * (N + M / 30); normalizedMotion.Y = (double)M / (N + M) * (N + M / 30);
            switch (spin)
            {
                case 0:
                    normalizedMotion.X *= 1; normalizedMotion.Y *= 1;
                    break;
                case 1:
                    normalizedMotion.X *= 1; normalizedMotion.Y *= -1;
                    break;
                case 2:
                    normalizedMotion.X *= -1; normalizedMotion.Y *= -1;
                    break;
                case 3:
                    normalizedMotion.X *= -1; normalizedMotion.Y *= 1;
                    break;
            }

            if (lastSpin.ElapsedMilliseconds >= 120 || !lastSpin.IsRunning)
            {
                lastSpin.Restart();
                spin += 1;
                if (spin > 3)
                {
                    spin = 0;
                }
            }

            if (lastSpinMove.ElapsedMilliseconds >= 1920 || !lastSpinMove.IsRunning)
            {
                lastSpinMove.Restart();

                // Find random spot where there is some fluid (>10% density).
                do
                {
                    spinOffsetAddX = spinRandom.Next(-(N - xH - 2) / 2, (N - xH - 2) / 2);
                    spinOffsetAddY = spinRandom.Next(-(M - yH - 2) / 2, (M - yH - 2) / 2);
                } while (d[FluidMath.IX(N, offsetX + spinOffsetAddX, offsetY + spinOffsetAddY)] < 0.10f);
            }

            offsetX += spinOffsetAddX;
            offsetY += spinOffsetAddY;

            for (int i = offsetX; i <= offsetX + xH - 1; i++)
            {
                for (int j = offsetY; j <= offsetY + yH - 1; j++)
                {
                    if (d[FluidMath.IX(N, i, j)] > 0)
                    {
                        addU = gravity * (float)normalizedMotion.X * d[FluidMath.IX(N, i, j)];
                        addV = gravity * (float)normalizedMotion.Y * d[FluidMath.IX(N, i, j)];
                    }
                    else
                    {
                        addU = gravity * (float)normalizedMotion.X;
                        addV = gravity * (float)normalizedMotion.Y;
                    }
                    uS[FluidMath.IX(N, i, j)] = addU;
                    vS[FluidMath.IX(N, i, j)] = addV;

                    // Set u.
                    cellVelocity = u[FluidMath.IX(N, i, j)] + addU;
                    if (cellVelocity < -terminalVelocity)
                    {
                        uS[FluidMath.IX(N, i, j)] = addU - (cellVelocity - -terminalVelocity);
                    }
                    else if (cellVelocity > terminalVelocity)
                    {
                        uS[FluidMath.IX(N, i, j)] = addU - (cellVelocity - terminalVelocity);
                    }
                    else uS[FluidMath.IX(N, i, j)] = addU;

                    // Set v.
                    cellVelocity = v[FluidMath.IX(N, i, j)] + addV;
                    if (cellVelocity < -terminalVelocity)
                    {
                        vS[FluidMath.IX(N, i, j)] = addV - (cellVelocity - -terminalVelocity); ;
                    }
                    else if (cellVelocity > terminalVelocity)
                    {
                        vS[FluidMath.IX(N, i, j)] = addV - (cellVelocity - terminalVelocity);
                    }
                    else vS[FluidMath.IX(N, i, j)] = addV;
                }
            }
        }

        void SimulateStep()
        {
            FluidMath.VelStep(N, M, u, v, uS, vS, viscosity, 1.0f);
            FluidMath.DensStep(N, M, d, dS, u, v, diffusion, 1.0f);

            // Clear source arrays.
            for (int i = 0; i < size; i++)
            {
                dS[i] = 0; uS[i] = 0; vS[i] = 0;
            }
        }
    }
}
