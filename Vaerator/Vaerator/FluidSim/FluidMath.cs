using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vaerator.FluidSim
{
    /// <summary>
    /// Based on Jos Stam's paper Real-Time Fluid Dynamics for Games.
    /// </summary>
    public static class FluidMath
    {
        public static float ComputeConservation(int N, int M, float[] d)
        {
            float total = 0;
            for(int i = 1; i <= N; i++)
            {
                for(int j = 1; j <= M; j++)
                {
                    total += Math.Abs(d[IX(N, i, j)]);
                }
            }
            return total;
        }

        public static void ApplyConservation(int N, int M, float[] d, float total, float total0)
        {
            float correction = total0 / total;
            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= M; j++)
                {
                    d[IX(N, i, j)] *= correction;
                }
            }
        }

        /// <summary>
        /// Computes a velocity step.
        /// </summary>
        /// <param name="N">Number of cells in a row (excluding boundary cells which are accounted for).</param>
        /// <param name="M">Number of cells in a column (excluding boundary cells which are accounted for).</param>
        /// <param name="u">Velocity horizontal component array.</param>
        /// <param name="v">Velocity vertical component array.</param>
        /// <param name="uS">Velocity source horizontal component array.</param>
        /// <param name="vS">Velocity source vertical component array.</param>
        /// <param name="visc">Viscosity (rate of velocity change).</param>
        /// <param name="dt">Delta time (step size).</param>
        public static void VelStep(int N, int M, float[] u, float[] v, float[] uS, float[] vS, float visc, float dt)
        {
            Task ASU = Task.Run(() => AddSource(N, M, u, uS, dt));
            Task ASV = Task.Run(() => AddSource(N, M, v, vS, dt));
            Task.WaitAll(ASU, ASV);

            //float massU0 = Task.Run(() => ComputeConservation(N, M, u)).Result;
            //float massV0 = Task.Run(() => ComputeConservation(N, M, v)).Result;

            Swap(ref uS, ref u);
            Swap(ref vS, ref v);

            if (visc > 0)
            {
                Task DFU = Task.Run(() => Diffuse(N, M, 1, u, uS, visc, dt, 20));
                Task DFV = Task.Run(() => Diffuse(N, M, 2, v, vS, visc, dt, 20));
                Task.WaitAll(DFU, DFV);
            }

            else
            {
                for (int i = 1; i <= N; i++)
                {
                    for (int j = 1; j <= M; j++)
                    {
                        uS[IX(N, i, j)] = u[IX(N, i, j)];
                        vS[IX(N, i, j)] = v[IX(N, i, j)];
                    }
                }
            }

            Project(N, M, u, v, uS, vS);

            Swap(ref uS, ref u);
            Swap(ref vS, ref v);

            Advect(N, M, 1, u, uS, uS, vS, dt);
            Advect(N, M, 2, v, vS, uS, vS, dt);

            Project(N, M, u, v, uS, vS);

            //float massU = Task.Run(() => ComputeConservation(N, M, u)).Result;
            //float massV = Task.Run(() => ComputeConservation(N, M, v)).Result;
            //Task CU = Task.Run(() => ApplyConservation(N, M, u, massU, massU0));
            //Task CV = Task.Run(() => ApplyConservation(N, M, v, massV, massV0));
            //Task.WaitAll(CU, CV);
        }

        /// <summary>
        /// Computes a density step.
        /// </summary>
        /// <param name="N">Number of cells in a row (excluding boundary cells which are accounted for).</param>
        /// <param name="M">Number of cells in a column (excluding boundary cells which are accounted for).</param>
        /// <param name="d">Density array.</param>
        /// <param name="dS">Density source array.</param>
        /// <param name="u">Velocity horizontal component array.</param>
        /// <param name="v">Velocity vertical component array.</param>
        /// <param name="diff">Rate of diffusion.</param>
        /// <param name="dt">Delta time (step size).</param>
        public static void DensStep(int N, int M, float[] d, float[] dS, float[] u, float[] v, float diff, float dt)
        {
            AddSource(N, M, d, dS, dt); // Add new source of density.
            float mass0 = ComputeConservation(N, M, d);
            if (diff > 0)
            {
                Swap(ref dS, ref d); // Make source our new array to fill.
                Diffuse(N, M, 0, d, dS, diff, dt, 20); // Diffuse.
                Swap(ref dS, ref d); // Swap back to normal.
            }

            else
            {
                for (int i = 1; i <= N; i++)
                {
                    for (int j = 1; j <= M; j++)
                    {
                        dS[IX(N, i, j)] = d[IX(N, i, j)];
                    }
                }
            }
            Advect(N, M, 0, d, dS, u, v, dt); // Advect.
            float mass = ComputeConservation(N, M, d);
            ApplyConservation(N, M, d, mass, mass0);
        }

        /// <summary>
        /// Maps the 2-D index to the corresponding 1-D index.
        /// </summary>
        /// <param name="N">Number of cells in a row (excluding boundary cells which are accounted for).</param>
        /// <param name="i">Cell column index.</param>
        /// <param name="j">Cell row index.</param>
        /// <returns>An index for the 1-D array.</returns>
        public static int IX(int N, int i, int j)
        {
            return i + ((N + 2) * j);
        }

        /// <summary>
        /// Adds new "source" components (velocity, density, etc.) to existing cells.
        /// </summary>
        /// <param name="N">Number of cells in a row (excluding boundary cells which are accounted for).</param>
        /// <param name="M">Number of cells in a column (excluding boundary cells which are accounted for).</param>
        /// <param name="x">Target array which is added to.</param>
        /// <param name="s">Source array.</param>
        /// <param name="dt">Delta time (step size).</param>
        private static void AddSource(int N, int M, float[] x, float[] s, float dt)
        {
            int size = (N + 2) * (M + 2);
            for (int i = 0; i < size; i++) x[i] += dt * s[i];
        }

        /// <summary>
        /// Swaps 2 array references.
        /// </summary>
        /// <param name="x">Array 1.</param>
        /// <param name="x0">Array 2.</param>
        private static void Swap(ref float[] x1, ref float[] x2)
        {
            float[] tmp = x2;
            x2 = x1;
            x1 = tmp;
        }

        /// <summary>
        /// Sets the values of the outer perimeter (boundary) cells.
        /// </summary>
        /// <param name="N">Number of cells in a row (excluding boundary cells which are accounted for).</param>
        /// <param name="M">Number of cells in a column (excluding boundary cells which are accounted for).</param>
        /// <param name="b">Boundary conditional (should be 0, 1, 2).</param>
        /// <param name="x">The 1-D array of cells.</param>
        private static void SetBnd(int N, int M, int b, float[] x)
        {
            // Top and bottom rows. No corners.
            for (int i = 1; i <= N; i++)
            {
                x[IX(N, i, 0)] = (b == 2) ? -x[IX(N, i, 1)] : x[IX(N, i, 1)]; // Bottom row.
                x[IX(N, i, M + 1)] = (b == 2) ? -x[IX(N, i, M)] : x[IX(N, i, M)]; // Top row.
            }
            
            // Left and right columns. No corners.
            for (int j = 1; j <= M; j++) {
                x[IX(N, 0, j)] = (b == 1) ? -x[IX(N, 1, j)] : x[IX(N, 1, j)]; // Left column.
                x[IX(N, N + 1, j)] = (b == 1) ? -x[IX(N, N, j)] : x[IX(N, N, j)]; // Right column.
            }

            // Corners.
            x[IX(N, 0, 0)] = 0.5f * (x[IX(N, 1, 0)] + x[IX(N, 0, 1)]); // Bottom left.
            x[IX(N, 0, M + 1)] = 0.5f * (x[IX(N, 1, M + 1)] + x[IX(N, 0, M)]); // Top left.
            x[IX(N, N + 1, 0)] = 0.5f * (x[IX(N, N, 0)] + x[IX(N, N + 1, 1)]); // Bottom right.
            x[IX(N, N + 1, M + 1)] = 0.5f * (x[IX(N, N, M + 1)] + x[IX(N, N + 1, M)]); // Top right.
        }

        /// <summary>
        /// Solves problem of exchanging each cell's value with that of its adjecent neighbours (stable solution).
        /// </summary>
        /// <param name="N">Number of cells in a row (excluding boundary cells which are accounted for).</param>
        /// <param name="M">Number of cells in a column (excluding boundary cells which are accounted for).</param>
        /// <param name="b">Boundary conditional (should be 0, 1, 2).</param>
        /// <param name="x">New array.</param>
        /// <param name="x0">Original array.</param>
        /// <param name="a">Multiply each neighbor by this.</param>
        /// <param name="c">Divide total by this.</param>
        /// <param name="kLimit">Max iterations for solver.</param>
        /// <param name="oscillate">Allows change of looping directions (good for diffusion).</param>
        private static void LinSolve(int N, int M, int b, float[] x, float[] x0, float a, float c, int kLimit, bool oscillate = true)
        {
            bool forward = true;

            for (int k = 0; k < kLimit; k++) //  WAS 20
            {
                if (forward || !oscillate)
                {
                    for (int i = 1; i <= N; i++)
                    {
                        for (int j = 1; j <= M; j++)
                        {
                            x[IX(N, i, j)] = (x0[IX(N, i, j)] + (a * (x[IX(N, i - 1, j)] + x[IX(N, i + 1, j)] + x[IX(N, i, j - 1)] + x[IX(N, i, j + 1)]))) / c;
                        }
                    }
                    forward = false;
                }
                else
                {
                    for (int i = N; i >= 1; i--)
                    {
                        for (int j = M; j >= 1; j--)
                        {
                            x[IX(N, i, j)] = (x0[IX(N, i, j)] + (a * (x[IX(N, i - 1, j)] + x[IX(N, i + 1, j)] + x[IX(N, i, j - 1)] + x[IX(N, i, j + 1)]))) / c;
                        }
                    }
                    forward = true;
                }
                SetBnd(N, M, b, x);
            }
        }

        /// <summary>
        /// Computes diffusion.
        /// </summary>
        /// <param name="N">Number of cells in a row (excluding boundary cells which are accounted for).</param>
        /// <param name="M">Number of cells in a column (excluding boundary cells which are accounted for).</param>
        /// <param name="b">Boundary conditional (should be 0, 1, 2).</param>
        /// <param name="x">New array.</param>
        /// <param name="x0">Original array.</param>
        /// <param name="diff">Rate of diffusion.</param>
        /// <param name="dt">Delta time (step size).</param>
        private static void Diffuse(int N, int M, int b, float[] x, float[] x0, float diff, float dt, int kLimit)
        {
            float a = dt * diff * N * M;
            LinSolve(N, M, b, x, x0, a, 1 + (4 * a), kLimit, true);
        }

        /// <summary>
        /// Moves density/velocity over velocity field.
        /// </summary>
        /// <param name="N">Number of cells in a row (excluding boundary cells which are accounted for).</param>
        /// <param name="M">Number of cells in a column (excluding boundary cells which are accounted for).</param>
        /// <param name="b">Boundary conditional (should be 0, 1, 2).</param>
        /// <param name="d">Array to compute.</param>
        /// <param name="d0">Input to advect.</param>
        /// <param name="u">Velocity horizontal component array.</param>
        /// <param name="v">Velocity vertical component array.</param>
        /// <param name="dt">Delta time (step size).</param>
        private static void Advect(int N, int M, int b, float[] d, float[] d0, float[] u, float[] v, float dt)
        {
            int i0, j0, i1, j1;
            float x, y, s0, t0, s1, t1;

            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= M; j++)
                {
                    x = i - (dt * N * u[IX(N, i, j)]); y = j - (dt * M * v[IX(N, i, j)]);
                    if (x < 0.5f) x = 0.5f; if (x > N + 0.5f) x = N + 0.5f; i0 = (int)x; i1 = i0 + 1;
                    if (y < 0.5f) y = 0.5f; if (y > M + 0.5f) y = M + 0.5f; j0 = (int)y; j1 = j0 + 1;
                    s1 = x - i0; s0 = 1 - s1; t1 = y - j0; t0 = 1 - t1;
                    d[IX(N, i, j)] = (s0 * ((t0 * d0[IX(N, i0, j0)]) + (t1 * d0[IX(N, i0, j1)]))) + (s1 * ((t0 * d0[IX(N, i1, j0)]) + (t1 * d0[IX(N, i1, j1)])));
                }
            }

            SetBnd(N, M, b, d);
        }

        /// <summary>
        /// Moves density/velocity over velocity field.
        /// </summary>
        /// <param name="N">Number of cells in a row (excluding boundary cells which are accounted for).</param>
        /// <param name="M">Number of cells in a column (excluding boundary cells which are accounted for).</param>
        /// <param name="u">Velocity horizontal component array.</param>
        /// <param name="v">Velocity vertical component array.</param>
        /// <param name="p">Array to compute. Can be blank, should be equal size to div.</param>
        /// <param name="div">Should be velocity vertical component array to compute height field.</param>
        private static void Project(int N, int M, float[] u, float[] v, float[] p, float[] div)
        {
            int i, j;
            float uH = (1.0f / N);
            float vH = (1.0f / M);

            for (i = 1; i <= N; i++)
            {
                for (j = 1; j <= M; j++)
                {
                    div[IX(N, i, j)] = (uH * -0.5f * ( u[IX(N, i + 1, j)] - u[IX(N, i - 1, j)])) +( vH * -0.5f * (v[IX(N, i, j + 1)] - v[IX(N, i, j - 1)]));
                    p[IX(N, i, j)] = 0;
                }
            }

            Task BDIV = Task.Run(() => SetBnd(N, M, 0, div));
            Task BP = Task.Run(() => SetBnd(N, M, 0, p));
            Task.WaitAll(BDIV, BP);

            LinSolve(N, M, 0, p, div, 1, 4, 20, true);

            for (i = 1; i <= N; i++)
            {
                for (j = 1; j <= M; j++)
                {
                    u[IX(N, i, j)] -= 0.5f * (p[IX(N, i + 1, j)] - p[IX(N, i - 1, j)]) / uH ; 
                    v[IX(N, i, j)] -= 0.5f * (p[IX(N, i, j + 1)] - p[IX(N, i, j - 1)]) / vH ;
                }
            }

            Task BDU = Task.Run(() => SetBnd(N, M, 1, u));
            Task BDV = Task.Run(() => SetBnd(N, M, 2, v));
            Task.WaitAll(BDU, BDV);
        }
    }
}
