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

        public static Parabola ParabolaFrom3Points(Point3d p1, Point3d p2, Point3d p3, Plane plane, Interval domain)
        {
            Point3dList ptList = new Point3dList(new[] { p1, p2, p3 });
            Point3dList ptListXform = new Point3dList(ptList);

            // compute points in WorldXY space
            if (plane != Plane.Unset && plane != Plane.WorldXY)
            {
                Transform transform = Geometry.WorldXYToPlaneTransform(plane);
                transform.TryGetInverse(out Transform inverseTransform);

                ptListXform.Transform(inverseTransform);
            }

            double[][] matrixValues = new double[3][];

            Vector<double> vector = Vector<double>.Build.Dense(new double[] { ptListXform[0].Y, ptListXform[1].Y, ptListXform[2].Y });

            for (int i = 0; i < matrixValues.Length; i++)
            {
                matrixValues[i] = new[] { ptListXform[i].X * ptListXform[i].X, ptListXform[i].X, 1 };
            }

            Matrix<double> matrix = DenseMatrix.OfRowArrays(matrixValues);

            Vector<double> quadratic = matrix.Solve(vector);

            // compute bounds if domain is unset
            if (domain == Interval.Unset)
                domain = ptListXform.ComputeBounds();

            var planeAngle = (plane.PlaneAngle2d() + 2 * Math.PI) % (2 * Math.PI);

            if (3 * Math.PI / 4 < planeAngle && planeAngle < 7 * Math.PI / 4)
                domain = new Interval(-domain.Max, -domain.Min);

            return new Parabola(quadratic[0], quadratic[1], quadratic[2], plane, domain);
        }

        public static Parabola[] ParabolaFrom4Points(IEnumerable<Point3d> pts)
        {
            // https://www.mathpages.com/home/kmath037/kmath037.htm
            // https://math.stackexchange.com/a/3224627

            Point3dList pList = new Point3dList(pts);

            if (pList.Count != 4)
                throw new ArgumentException("Incorrect number of points specified for this method");

            // this method requires a transformation such that
            // p3 is at the origin (0, 0, 0)
            // and p4 is at (1, 0, 0)
            // this unitizes the problem such that we can solve across p1 and p2

            // create new points which meet the translation, rotation and scale requirements to solve
            var transformedPoints = GetTransformedPoints(pList);
            Point3d p1xForm = transformedPoints[0];
            Point3d p2xForm = transformedPoints[1];

            // we calculate the angle quadratic of the form A*tan(t)^2 + B*Tan(t) + C = 0
            double a = p2xForm.Y - p1xForm.Y;
            double b = 2 * (p2xForm.X - p1xForm.X);
            double c = p2xForm.X * (p2xForm.X - 1) / p2xForm.Y - p1xForm.X * (p1xForm.X - 1) / p1xForm.Y;

            // the angle of p4-p3 gives us the transformation.
            double rotation = (pList[3] - pList[2]).VectorAngle();

            // calculating the roots gives us the solution tan(t) = root1, root2
            // sp we take the inverse Tan to find the angle at which valid parabolae will form
            var roots = Geometry.ComputeQuadraticRoots(a, b, c);

            if (roots.Length < 2)
                return new Parabola[] { null, null };

            double ang1 = Math.Atan(roots[0]) + rotation;
            double ang2 = Math.Atan(roots[1]) + rotation;

            // create planes which meet the angle requirement
            Plane plane1 = Plane.WorldXY;
            plane1.Rotate(ang1, plane1.ZAxis);

            Plane plane2 = Plane.WorldXY;
            plane2.Rotate(ang2, plane2.ZAxis);

            // calculate the domain in the given plane
            Interval domain1 = pList.ComputeTransformedBoundsInPlane(plane1, Plane.WorldXY);
            Interval domain2 = pList.ComputeTransformedBoundsInPlane(plane2, Plane.WorldXY);

            // construct the parabolas from three of the points and the calculated plane
            Parabola parabola1 = ParabolaFrom3Points(pList[0], pList[1], pList[3], plane1, domain1);
            Parabola parabola2 = ParabolaFrom3Points(pList[0], pList[1], pList[3], plane2, domain2);

            return new Parabola[] { parabola1, parabola2 };
        }

        private static Point3dList GetTransformedPoints(Point3dList pts)
        {
            Vector3d translationVector = Point3d.Origin - pts[2];
            Transform translation = Transform.Translation(translationVector);

            double rotationAngle = -(pts[3] - pts[2]).VectorAngle();
            Transform rotation = Transform.Rotation(rotationAngle, Point3d.Origin);

            double transformScale = 1 / (pts[3] - pts[2]).Length;
            Transform scale = Transform.Scale(Point3d.Origin, transformScale);

            var ptsTransformed = new Point3dList(pts);
            ptsTransformed.Transform(translation);
            ptsTransformed.Transform(rotation);
            ptsTransformed.Transform(scale);

            return ptsTransformed;
        }
    }
}
