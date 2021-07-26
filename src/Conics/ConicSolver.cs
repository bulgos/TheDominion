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
        //public static void SolveConic(Point3d pt1, Point3d pt2, Point3d pt3, Point3d pt4, Point3d pt5)
        //{
        //    Matrix<double> matrix = DenseMatrix.OfArray(new double[,]{
        //        {Math.Pow(pt1.X,2), pt1.X * pt1.Y, Math.Pow(pt1.Y,2), pt1.X, pt1.Y, 1},
        //        {Math.Pow(pt2.X,2), pt2.X * pt2.Y, Math.Pow(pt2.Y,2), pt2.X, pt2.Y, 1},
        //        {Math.Pow(pt3.X,2), pt3.X * pt3.Y, Math.Pow(pt3.Y,2), pt3.X, pt3.Y, 1},
        //        {Math.Pow(pt4.X,2), pt4.X * pt4.Y, Math.Pow(pt4.Y,2), pt4.X, pt4.Y, 1},
        //        {Math.Pow(pt5.X,2), pt5.X * pt5.Y, Math.Pow(pt5.Y,2), pt5.X, pt5.Y, 1}
        //    });


        //}
        private Matrix<double> m_xx = DenseMatrix.OfArray(new double[5, 5]);
        private Matrix<double> m_x = DenseMatrix.OfArray(new double[5, 1]);

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
                { p.X * p.X, p.X * p.Y, p.Y * p.Y, p.X, p.Y }
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

        void Solve(ref double a, ref double b, ref double c, ref double d, ref double e, ref double f)
        {
            Matrix<double> y = m_xx.Cholesky().Solve(m_x);
            a = y[0,0];
            b = y[0,1] * 0.5;
            c = y[0,2];
            d = y[0,3] * 0.5;
            e = y[0,4] * 0.5;
            f = -1.0;
        }

        void SolveDual(ref double a, ref double b, ref double c, ref double d, ref double e, ref double f)
        {
            Solve(ref a, ref b, ref c, ref d, ref e, ref f);
            Matrix<double> m; = DenseMatrix.OfArray(new double[3, 3]
            {
                { a, b, d},
                { b, c, e},
                { d, e, f}
            });

            var minv = -m.Inverse();
            a = minv[0, 0];
            b = minv[0, 1];
            c = minv[1, 1];
            d = minv[0, 2];
            e = minv[1, 2];
            f = minv[2, 2];
        }
    }
}
