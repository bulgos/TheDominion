using Dominion.Core.Utility;
using MathNet.Numerics.LinearAlgebra;
using Rhino.Collections;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Dominion.Core.Conics
{
    public static class ConicFactory
    {
        public static ConicSection From4Points(IEnumerable<Point3d> points)
        {
            return From4Points(points, Plane.WorldXY);
        }

        public static ConicSection From4Points(IEnumerable<Point3d> points, Plane plane)
        {
            Point3dList pts = new Point3dList(points);

            Transform xform = Geometry.WorldXYToPlaneTransform(plane);
            xform.TryGetInverse(out Transform xformInverse);
            pts.Transform(xformInverse);

            if (pts.Count != 4)
                throw new ArgumentException("Incorrect number of points specified");

            double[][] matrixValues = new double[4][];
            Vector<double> vector = Vector.Build.Dense(4, 1);

            for (int i = 0; i < pts.Count; i++)
            {
                matrixValues[i] = new[] { pts[i].X * pts[i].X, pts[i].Y * pts[i].Y, pts[i].X, pts[i].Y };
            }

            Matrix<double> matrix = DenseMatrix.OfRowArrays(matrixValues);

            Vector<double> solution = matrix.Solve(vector);

            double a = solution[0];
            double c = solution[1];
            double d = solution[2];
            double e = solution[3];
            double f = -1;

            ConicSection conic = FromConicEquation(a, 0, c, d, e, f);

            conic.Transform(xform);

            return conic;
        }

        public static ConicSection From5Points(IEnumerable<Point3d> points)
        {
            // simplest solution we could find
            // https://math.stackexchange.com/a/1987192/951797

            var pts = points.ToArray();

            if (pts.Length != 5)
                throw new ArgumentException("Incorrect number of points specified");

            double[][] matrixValues = new double[5][];
            Vector<double> vector = Vector.Build.Dense(5, 1);

            for (int i = 0; i < pts.Length; i++)
            {
                matrixValues[i] = new[] { pts[i].X * pts[i].X, pts[i].X * pts[i].Y, pts[i].Y * pts[i].Y, pts[i].X, pts[i].Y };
            }

            Matrix<double> matrix = DenseMatrix.OfRowArrays(matrixValues);

            Vector<double> solution = matrix.Solve(vector);


            double a = solution[0];
            double b = solution[1];
            double c = solution[2];
            double d = solution[3];
            double e = solution[4];
            double f = -1;

            return FromConicEquation(a, b, c, d, e, f);
        }

        public static ConicSection FromConicEquation(double a, double b, double c, double d, double e, double f)
        {
            return FromConicEquation(Plane.WorldXY, a, b, c, d, e, f);
        }

        public static ConicSection FromConicEquation(Plane plane, double a, double b, double c, double d, double e, double f)
        {
            ConicSection conicSection = new ConicSection(plane, a, b, c, d, e, f);

            switch (conicSection.ConicSectionType)
            {
                case ConicSectionType.Circle:
                    return new Ellipse(conicSection);
                case ConicSectionType.Ellipse:
                    return new Ellipse(conicSection);
                case ConicSectionType.Hyperbola:
                    return new Hyperbola(conicSection);
                case ConicSectionType.Parabola:
                    return new Parabola(conicSection);
                default:
                    return conicSection;
            }
        }
    }
}
