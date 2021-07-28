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
    }
}
