using System;
using System.Collections.Generic;
using System.Text;

namespace Vaerator.FluidSolver
{
    /// <summary>
    /// Based on Jos Stam's paper Real-Time Fluid Dynamics for Games.
    /// </summary>
    public static class FluidMath
    {
        /// <summary>
        /// Computes a velocity step.
        /// </summary>
        /// <param name="N">Number of cells in a row (excluding boundary cells which are accounted for).</param>
        /// <param name="M">Number of cells in a column (excluding boundary cells which are accounted for).</param>
        /// <param name="u">Velocity horizontal component array.</param>
        /// <param name="u">Velocity vertical component array.</param>
        /// <param name="uS">Velocity source horizontal component array.</param>
        /// <param name="vS">Velocity source vertical component array.</param>
        /// <param name="visc">Viscosity (rate of velocity change).</param>
        /// <param name="dt">Delta time (step size).</param>
        public static void VelStep(int N, int M, ref float[] u, ref float[] v, ref float[] uS, ref float[] vS, float visc, float dt)
        {
            AddSource(N, M, ref u, ref uS, dt);
            AddSource(N, M, ref v, ref vS, dt);
            Swap(ref uS, ref u);
            Diffuse(N, M, 1, ref u, ref uS, visc, dt);
            Swap(ref vS, ref v);
            Diffuse(N, M, 2, ref v, ref vS, visc, dt);
            Project(N, M, ref u, ref v, ref uS, ref vS);
            Swap(ref uS, ref u);
            Swap(ref vS, ref v);
            Advect(N, M, 1, ref u, ref uS, ref uS, ref vS, dt);
            Advect(N, M, 2, ref v, ref vS, ref uS, ref vS, dt);
            Project(N, M, ref u, ref v, ref uS, ref vS);
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
        public static void DensStep(int N, int M, ref float[] d, ref float[] dS, ref float[] u, ref float[] v, float diff, float dt)
        {
            AddSource(N, M, ref d, ref dS, dt); // Add new source of density.
            Swap(ref dS, ref d); // Make source our new array to fill.
            Diffuse(N, M, 0, ref d, ref dS, diff, dt); // Diffuse.
            Swap(ref dS, ref d); // Swap back to normal.
            Advect(N, M, 0, ref d, ref dS, ref u, ref v, dt); // Advect.
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
        private static void AddSource(int N, int M, ref float[] x, ref float[] s, float dt)
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
        private static void SetBnd(int N, int M, int b, ref float[] x)
        {
            // Top and bottom rows. No corners.
            for (int i = 1; i <= N; i++)
            {
                x[IX(N, i, 0)] = b == 2 ? -x[IX(N, i, 1)] : x[IX(N, i, 1)];
                x[IX(N, i, N + 1)] = b == 2 ? -x[IX(N, i, N)] : x[IX(N, i, N)];
            }
            
            // Left and right columns. No corners.
            for (int j = 1; j <= M; j++) {
                x[IX(N, 0, j)] = b == 1 ? -x[IX(N, 1, j)] : x[IX(N, 1, j)];
                x[IX(N, N + 1, j)] = b == 1 ? -x[IX(N, N, j)] : x[IX(N, N, j)];
            }

            // Corners.
            x[IX(N, 0, 0)] = 0.5f * (x[IX(N, 1, 0)] + x[IX(N, 0, 1)]);
            x[IX(N, 0, M + 1)] = 0.5f * (x[IX(N, 1, N + 1)] + x[IX(N, 0, N)]);
            x[IX(N, N + 1, 0)] = 0.5f * (x[IX(N, N, 0)] + x[IX(N, N + 1, 1)]);
            x[IX(N, N + 1, N + 1)] = 0.5f * (x[IX(N, N, N + 1)] + x[IX(N, N + 1, N)]);
        }

        /// <summary>
        /// Solves problem of exchanging each cell's value with that of its adjecent neighbours (stable solution).
        /// </summary>
        /// <param name="N">Number of cells in a row (excluding boundary cells which are accounted for).</param>
        /// <param name="M">Number of cells in a column (excluding boundary cells which are accounted for).</param>
        /// <param name="b">Boundary conditional (should be 0, 1, 2).</param>
        /// <param name="x">New array.</param>
        /// <param name="x0">Original array.</param>
        /// <param name="a"></param>
        /// <param name="c"></param>
        private static void LinSolve(int N, int M, int b, ref float[] x, ref float[] x0, float a, float c)
        {
            for (int k = 0; k < 20; k++)
            {
                for (int i = 1; i <= N; i++)
                {
                    for (int j = 1; j <= M; j++)
                    {
                        x[IX(N, i, j)] = (x0[IX(N, i, j)] + a * (x[IX(N, i - 1, j)] + x[IX(N, i + 1, j)] + x[IX(N, i, j - 1)] + x[IX(N, i, j + 1)])) / c;
                    }
                }

                SetBnd(N, M, b, ref x);
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
        private static void Diffuse(int N, int M, int b, ref float[] x, ref float[] x0, float diff, float dt)
        {
            float a = dt * diff * N * M;
            LinSolve(N, M, b, ref x, ref x0, a, 1 + (4 * a));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="N"></param>
        /// <param name="M"></param>
        /// <param name="b"></param>
        /// <param name="d"></param>
        /// <param name="d0"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="dt"></param>
        private static void Advect(int N, int M, int b, ref float[] d, ref float[] d0, ref float[] u, ref float[] v, float dt)
        {
            int i0, j0, i1, j1;
            float x, y, s0, t0, s1, t1, dt0;

            dt0 = dt * N;
            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= M; j++)
                {
                    x = i - dt0 * u[IX(N, i, j)]; y = j - dt0 * v[IX(N, i, j)];
                    if (x < 0.5f) x = 0.5f; if (x > N + 0.5f) x = N + 0.5f; i0 = (int)x; i1 = i0 + 1;
                    if (y < 0.5f) y = 0.5f; if (y > N + 0.5f) y = N + 0.5f; j0 = (int)y; j1 = j0 + 1;
                    s1 = x - i0; s0 = 1 - s1; t1 = y - j0; t0 = 1 - t1;
                    d[IX(N, i, j)] = s0 * (t0 * d0[IX(N, i0, j0)] + t1 * d0[IX(N, i0, j1)]) + s1 * (t0 * d0[IX(N, i1, j0)] + t1 * d0[IX(N, i1, j1)]);
                }
            }

            SetBnd(N, M, b, ref d);
        }

        private static void Project(int N, int M, ref float[] u, ref float[] v, ref float[] p, ref float[] div)
        {
            int i, j;
            float uH = (1.0f / N) * -0.5f;
            float vH = (1.0f / M) * -0.5f;

            for (i = 1; i <= N; i++)
            {
                for (j = 1; j <= M; j++)
                {
                    div[IX(N, i, j)] = (uH * u[IX(N, i + 1, j)]) - (uH * u[IX(N, i - 1, j)]) + (vH * v[IX(N, i, j + 1)]) - (vH * v[IX(N, i, j - 1)]);
                    p[IX(N, i, j)] = 0;
                }
            }

            SetBnd(N, M, 0, ref div);
            SetBnd(N, M, 0, ref p);

            LinSolve(N, M, 0, ref p, ref div, 1, 4);

            for (i = 1; i <= N; i++)
            {
                for (j = 1; j <= M; j++)
                {
                    u[IX(N, i, j)] -= 0.5f * N * (p[IX(N, i + 1, j)] - p[IX(N, i - 1, j)]);
                    v[IX(N, i, j)] -= 0.5f * M * (p[IX(N, i, j + 1)] - p[IX(N, i, j - 1)]);
                }
            }

            SetBnd(N, M, 1, ref u);
            SetBnd(N, M, 2, ref v);
        }
    }
}
