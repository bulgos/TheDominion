using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace the_Dominion.Conics
{
    public class ConicSolver
    {
        private Matrix<double> m_xx = DenseMatrix.OfArray(new double[5, 5]);
        private Matrix<double> m_x = DenseMatrix.OfArray(new double[5, 1]);

        public double A { get; private set; }

        public double B { get; private set; }

        public double C { get; private set; }

        public double D { get; private set; }

        public double E { get; private set; }

        public double F { get; private set; }

        public double Determinant { get; private set; }

        public ConicSolver() => Reset();

        void Reset()
        {
            m_xx.Clear();
            m_x.Clear();
        }

        void AddPoint(Point2d p)
        {
            Matrix<double> x = DenseMatrix.OfArray(new double[,]
            {
                { p.X * p.X },
                { p.X * p.Y },
                { p.Y * p.Y },
                { p.X },
                { p.Y }
            });

            m_x += x;
            m_xx += x * x.Transpose();
        }

        void AddDualLine(Point2d p1, Point2d p2)
        {
            Vector2d v = (p2 - p1);
            Vector2d v1 = new Vector2d(p1.X, p1.Y);
            v.Unitize();
            double d = Vector2d.Multiply(v, v1);

            Point2d t = (d * new Point2d(v - v1));
            var s = (Vector2d.Multiply(v1, v1) - d * d);

            var pt = t / s;
            AddPoint(pt);
        }

        void Solve()
        {
            //Matrix<double> y = m_xx.Cholesky().Solve(m_x);
            Matrix<double> y = m_xx.LU().Solve(m_x);
            A = y[0, 0];
            B = y[1, 0] * 0.5;
            C = y[2, 0];
            D = y[3, 0] * 0.5;
            E = y[4, 0] * 0.5;
            F = -1.0;

            ComputeDetemrinant();
        }

        void SolveDual()
        {
            Solve();
            Matrix<double> m = DenseMatrix.OfArray(new double[,]
            {
                { A, B, D},
                { B, C, E},
                { D, E, F}
            });

            var minv = -m.Inverse();
            A = minv[0, 0];
            B = minv[0, 1];
            C = minv[1, 1];
            D = minv[0, 2];
            E = minv[1, 2];
            F = minv[2, 2];
        }

        private void ComputeDetemrinant()
        {
            Determinant = B * B - (4 * A * C);
        }

        public static ConicSolver SolveConic(Point3d pt1, Point3d pt2, Point3d pt3, Point3d pt4, Point3d pt5)
        {
            //Matrix<double> matrix = DenseMatrix.OfArray(new double[,]{
            //    {Math.Pow(pt1.X,2), pt1.X * pt1.Y, Math.Pow(pt1.Y,2), pt1.X, pt1.Y, 1},
            //    {Math.Pow(pt2.X,2), pt2.X * pt2.Y, Math.Pow(pt2.Y,2), pt2.X, pt2.Y, 1},
            //    {Math.Pow(pt3.X,2), pt3.X * pt3.Y, Math.Pow(pt3.Y,2), pt3.X, pt3.Y, 1},
            //    {Math.Pow(pt4.X,2), pt4.X * pt4.Y, Math.Pow(pt4.Y,2), pt4.X, pt4.Y, 1},
            //    {Math.Pow(pt5.X,2), pt5.X * pt5.Y, Math.Pow(pt5.Y,2), pt5.X, pt5.Y, 1}
            //});

            Point2d p1 = new Point2d(pt1.X, pt1.Y);
            Point2d p2 = new Point2d(pt2.X, pt2.Y);
            Point2d p3 = new Point2d(pt3.X, pt3.Y);
            Point2d p4 = new Point2d(pt4.X, pt4.Y);
            Point2d p5 = new Point2d(pt5.X, pt5.Y);

            ConicSolver conicSolver = new ConicSolver();

            conicSolver.AddPoint(p1);
            conicSolver.AddPoint(p2);
            conicSolver.AddPoint(p3);
            conicSolver.AddPoint(p4);
            conicSolver.AddPoint(p5);

            conicSolver.Solve();

            return conicSolver;
        }

        public void From5Points(Point3d pt1, Point3d pt2, Point3d pt3, Point3d pt4, Point3d pt5)
        {
            Point2d p1 = new Point2d(pt1.X, pt1.Y);
            Point2d p2 = new Point2d(pt2.X, pt2.Y);
            Point2d p3 = new Point2d(pt3.X, pt3.Y);
            Point2d p4 = new Point2d(pt4.X, pt4.Y);
            Point2d p5 = new Point2d(pt5.X, pt5.Y);

            Point2d[] pts = new[] { p1, p2, p3, p4, p5 };

            double[][] matrixValues = new double[5][];
            Vector<double> vector = Vector.Build.Dense(5, 1);

            for (int i = 0; i < pts.Length; i++)
            {
                matrixValues[i] = new[] { pts[i].X * pts[i].X, pts[i].X * pts[i].Y, pts[i].Y * pts[i].Y, pts[i].X, pts[i].Y };
            }

            Matrix<double> matrix = DenseMatrix.OfRowArrays(matrixValues);

            Vector<double> solution = matrix.Solve(vector);

            A = solution[0];
            B = solution[1];
            C = solution[2];
            D = solution[3];
            E = solution[4];
            //F = solution[5, 0];
            F = -1;
        }

        public static ConicSolver FindConicSection(List<Point3d> points)
        {
            const int num_rows = 5;
            const int num_cols = 5;

            var conicSolver = new ConicSolver();

            // Build the augmented matrix.
            double[,] arr = new double[num_rows, num_cols + 2];
            for (int row = 0; row < num_rows; row++)
            {
                arr[row, 0] = points[row].X * points[row].X;
                arr[row, 1] = points[row].X * points[row].Y;
                arr[row, 2] = points[row].Y * points[row].Y;
                arr[row, 3] = points[row].X;
                arr[row, 4] = points[row].Y;
                arr[row, 5] = -1;
                arr[row, 6] = 0;
            }
            Console.WriteLine("    Initial Array:");

            // Perform Gaussian elmination.
            const double tiny = 0.00001;
            for (int r = 0; r < num_rows - 1; r++)
            {
                // Zero out all entries in column r after this row.
                // See if this row has a non-zero entry in column r.
                if (Math.Abs(arr[r, r]) < tiny)
                {
                    // Too close to zero. Try to swap with a later row.
                    for (int r2 = r + 1; r2 < num_rows; r2++)
                    {
                        if (Math.Abs(arr[r2, r]) > tiny)
                        {
                            // This row will work. Swap them.
                            for (int c = 0; c <= num_cols; c++)
                            {
                                double tmp = arr[r, c];
                                arr[r, c] = arr[r2, c];
                                arr[r2, c] = tmp;
                            }
                            break;
                        }
                    }
                }

                // If this row has a non-zero entry in column r, use it.
                if (Math.Abs(arr[r, r]) > tiny)
                {
                    // Zero out this column in later rows.
                    for (int r2 = r + 1; r2 < num_rows; r2++)
                    {
                        double factor = -arr[r2, r] / arr[r, r];
                        for (int c = r; c <= num_cols; c++)
                        {
                            arr[r2, c] = arr[r2, c] + factor * arr[r, c];
                        }
                    }
                }
                Console.WriteLine("    After eliminating column " + r + ":");
            }
            Console.WriteLine("    After elimination:");

            // See if we have a solution.
            if (arr[num_rows - 1, num_cols - 1] == 0)
            {
                // We have no solution.
                // See if all of the entries in this row are 0.
                bool all_zeros = true;
                for (int c = 0; c <= num_cols + 1; c++)
                {
                    if (arr[num_rows - 1, c] != 0)
                    {
                        all_zeros = false;
                        break;
                    }
                }
                if (all_zeros)
                {
                    // solution is not unique
                }
                else
                {
                    // no solution exists
                }
                conicSolver.A = 0;
                conicSolver.B = 0;
                conicSolver.C = 0;
                conicSolver.D = 0;
                conicSolver.E = 0;
                conicSolver.F = 0;
            }
            else
            {
                // Backsolve.
                for (int r = num_rows - 1; r >= 0; r--)
                {
                    double tmp = arr[r, num_cols];
                    for (int r2 = r + 1; r2 < num_rows; r2++)
                    {
                        tmp -= arr[r, r2] * arr[r2, num_cols + 1];
                    }
                    arr[r, num_cols + 1] = tmp / arr[r, r];
                }

                // Save the results.
                conicSolver.A = arr[0, num_cols + 1];
                conicSolver.B = arr[1, num_cols + 1];
                conicSolver.C = arr[2, num_cols + 1];
                conicSolver.D = arr[3, num_cols + 1];
                conicSolver.E = arr[4, num_cols + 1];
                conicSolver.F = 1;
            }

            return conicSolver;
        }
    }
}
