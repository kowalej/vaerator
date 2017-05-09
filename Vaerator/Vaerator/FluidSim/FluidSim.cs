using System;
using System.Diagnostics;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using System.Threading.Tasks;

namespace Vaerator.FluidSim
{
    public class FluidSimulation
    {
        public const int MAX_CELLS = 100;
        public const int MIN_CELLS = 20;
        public const float FORCE_COVERAGE = 0.08f;

        IFluidRenderer renderer;
        int N; // Number of horizontal cells.
        int M; // Number of vertical cells.
        float[] u, uS; // Horizontal velocity and velocity source components.
        float[] v, vS; // Vertical velocity and velocity source components.
        float[] d, dS; // Densisty and density source values.
        float dt; // How fast to run simulation (milliseconds between simulation steps).
        float resolution; // Resolution of renderering, from 0.0 to 1.0. The higher the resolution, the nice the simulation looks.
        volatile bool running;
        public bool Running { get { return running; }  } // Flag's whether or not the simulation is running.
        int size; // Size of velocity/density arrays.
        readonly float GRAVITY_CONSTANT, TERMINAL_VELOCITY_CONSTANT, VISCOSITY_CONSTANT, DIFFUSION_RATE_CONSTANT; // Model constants.
        float viscosity, diffusion, gravity, terminalVelocity; // Computed values.
        volatile MotionVector deviceMotion;

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
            gravity = GRAVITY_CONSTANT / (1000 / dt) * N * M;
            terminalVelocity = TERMINAL_VELOCITY_CONSTANT / (1000 / dt) * N * M;

            // Add initial density source.
            AddCenteredRect(0.5f, 0.5f);

            // Diffuse a little for rounded effect.
            diffusion = 0.003f;
            for (int i = 0; i < 2; i++)
            {
                SimulateStep();
            }

            // Normal diffusion rate.
            diffusion = DIFFUSION_RATE_CONSTANT;

            // Pass density to renderer so it knows what to draw.
            renderer.SetDensity(ref d);
        }

        void AddCenteredRect(float coverage, float densityFull)
        {
            int xH, yH;

            if (N % 2 == 0)
                xH = Helpers.Misc.RoundToEven(N * coverage); // Round to even.
            else xH = Helpers.Misc.RoundToOdd(N * coverage); // Round to odd.

            if (M % 2 == 0)
                yH = Helpers.Misc.RoundToEven(M * coverage); // Round to even.
            else yH = Helpers.Misc.RoundToOdd(M * coverage); // Round to odd.

            float fill = ((N * M) / (xH * yH)) * densityFull;

            int offSetX = 1 + ((N - xH) / 2);
            int offSetY = 1 + ((M - yH) / 2);

            for (int i = offSetX; i <= offSetX + xH - 1; i++)
            {
                for (int j = offSetY; j <= offSetY + yH - 1; j++)
                {
                    dS[FluidMath.IX(N, i, j)] = fill;
                }
            }
        }

        public async Task Start()
        {
            Stopwatch stopWatchFrame = new Stopwatch();
            running = true;
            StartMotionCapture();

            while (running)
            {
                if (stopWatchFrame.ElapsedMilliseconds >= dt || !stopWatchFrame.IsRunning)
                {
                    stopWatchFrame.Restart();

                    // Run simulation step.
                    SimulateStep();
                    AddForces();

                    if (stopWatchFrame.ElapsedMilliseconds > dt)
                    {
                        var elapsed = stopWatchFrame.ElapsedMilliseconds;
                        Debug.WriteLine("Slow frame time: {0} milliseconds.", elapsed);
                    }
                    await renderer.Render((int)Math.Max(1, dt - stopWatchFrame.ElapsedMilliseconds));
                }
            }
        }

        public void Stop()
        {
            running = false;
            StopMotionCapture();
        }

        void StartMotionCapture()
        {
            CrossDeviceMotion.Current.SensorValueChanged += AccelSensorValueChanged;
            CrossDeviceMotion.Current.Start(MotionSensorType.Accelerometer, MotionSensorDelay.Game);
        }

        void StopMotionCapture()
        {
            CrossDeviceMotion.Current.Stop(MotionSensorType.Accelerometer);
            CrossDeviceMotion.Current.SensorValueChanged -= AccelSensorValueChanged;
        }

        void AccelSensorValueChanged(object sender, SensorValueChangedEventArgs e)
        {
            if (e.SensorType == MotionSensorType.Accelerometer)
            {
                deviceMotion = (MotionVector)e.Value;
                #if DEBUG
                    Debug.WriteLine("X: {0}, Y: {1}, Z{2}", deviceMotion.X, deviceMotion.Y, deviceMotion.Z);
                #endif
            }
        }

        void AddForces()
        {
            diffusion = 0.0f; // DIFFUSION_RATE_CONSTANT / 100000f;
            float cellVelocity;

            float addU = 0, addV = 0;

            int xH, yH;

            if (N % 2 == 0)
                xH = Helpers.Misc.RoundToEven(N * FORCE_COVERAGE); // Round to even.
            else xH = Helpers.Misc.RoundToOdd(N * FORCE_COVERAGE); // Round to odd.

            if (M % 2 == 0)
                yH = Helpers.Misc.RoundToEven(M * FORCE_COVERAGE); // Round to even.
            else yH = Helpers.Misc.RoundToOdd(M * FORCE_COVERAGE); // Round to odd.

            int offSetX = 1 + ((N - xH) / 2);
            int offSetY = 1 + ((M - yH) / 2);

            // Normalize and reverse X & Y direction.
            MotionVector normalizedMotion = new MotionVector();
            double normal = 1.0d; //; Math.Sqrt(Math.Pow(deviceMotion.X, 2) + Math.Pow(deviceMotion.Y, 2) + Math.Pow(deviceMotion.Z, 2));
            normalizedMotion.X = deviceMotion.X / normal * -1.0d; normalizedMotion.Y = deviceMotion.Y / normal * -1.0d; normalizedMotion.Z = deviceMotion.Z / normal;

            for (int i = offSetX; i <= offSetX + xH - 1; i++)
            {
                for (int j = offSetY; j <= offSetY + yH - 1; j++)
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
