using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace the_Dominion
{
    public class Parabola : ConicSection
    {
        public Parabola(double a, double b, double c, Interval domain)
        {
            A = a;
            B = b;
            C = c;
            Domain = domain;

            ConstructParabola();
        }

        public Parabola(Plane plane, Point3d p1, Point3d p2, Point3d p3)
        {
            Transform xform = Transform.PlaneToPlane(Plane.WorldXY, plane);
            xform.TryGetInverse(out Transform xformInverse);

            p1.Transform(xformInverse);
            p2.Transform(xformInverse);
            p3.Transform(xformInverse);

            Tuple<double, double, double> quadratic = ComputeParabolaParametersFrom3Points(p1, p2, p3);

            A = quadratic.Item1;
            B = quadratic.Item2;
            C = quadratic.Item3;

            double[] xValues = { p1.X, p2.X, p3.X };
            
            Domain = new Interval(xValues.Min(), xValues.Max());

            ConstructParabola();

            TransformShape(xform);
        }

        public Parabola()
            : this(1, new Interval(-1, 1)) { }

        public Parabola(double a, Interval domain)
        {
            A = a;
            Domain = domain;

            ConstructParabola();
        }

        private double A { get; }

        public double B { get; }

        public double C { get; }

        public Interval Domain { get; } = new Interval(-1, 1);

        public void ConstructParabola()
        {
            Point3d p1 = ComputeParabolaPoint(Domain.Min);
            Point3d p2 = ComputeParabolaPoint(Domain.Max);

            Vector3d t1 = ComputeParabolaTangent(Domain.Min);
            Vector3d t2 = ComputeParabolaTangent(Domain.Max);

            Line tLine1 = new Line(p1, p1 + t1);
            Line tLine2 = new Line(p2, p2 + t2);

            Intersection.LineLine(tLine1, tLine2, out double param1, out double param2);

            Point3d p0 = tLine1.PointAt(param1);

            Point3d[] points = { p1, p0, p2 };

            Section = Curve.CreateControlPointCurve(points, 2) as NurbsCurve;

            ComputeBasePlane();
            ComputeFocus();
        }

        private Tuple<double, double, double> ComputeParabolaParametersFrom3Points(Point3d p1, Point3d p2, Point3d p3)
        {
            /// http://stackoverflow.com/questions/717762/how-to-calculate-the-vertex-of-a-parabola-given-three-points

            double denom = (p1.X - p2.X) * (p1.X - p3.X) * (p2.X - p3.X);
            double a = (p3.X * (p2.Y - p1.Y) + p2.X * (p1.Y - p3.Y) + p1.X * (p3.Y - p2.Y)) / denom;
            double b = (p3.X * p3.X * (p1.Y - p2.Y) + p2.X * p2.X * (p3.Y - p1.Y) + p1.X * p1.X * (p2.Y - p3.Y)) / denom;
            double c = (p2.X * p3.X * (p2.X - p3.X) * p1.Y + p3.X * p1.X * (p3.X - p1.X) * p2.Y + p1.X * p2.X * (p1.X - p2.X) * p3.Y) / denom;

            return new Tuple<double, double, double>(a, b, c);
        }

        /// <summary>
        /// Finds the line through which the Parabola mirrors
        /// </summary>
        /// <returns></returns>
        private Point3d ComputeParabolaVertex()
        {
            double x = -B / (2 * A);

            return ComputeParabolaPoint(x);
        }

        private Point3d ComputeParabolaPoint(double x)
        {
            double y = A * x * x + B * x + C;

            return new Point3d(x, y, 0);
        }

        private Vector3d ComputeParabolaTangent(double x)
        {
            double derivative = 2 * A * x + B;

            return new Vector3d(1, derivative, 0);
        }

        protected override void ComputeFocus()
        {
            Focus = BasePlane.Origin + new Point3d(0, A / 4, 0);
        }

        private void ComputeBasePlane()
        {
            var vertex = ComputeParabolaVertex();

            var basePlane = Plane.WorldXY;
            basePlane.Origin = vertex;

            if (A < 0)
            {
                basePlane.Rotate(Math.PI, basePlane.ZAxis);
            }

            BasePlane = basePlane;
        }

        public void ConstructParabolaFromFocus(double a, Interval interval)
        {
            ComputeFocus();

            double y0 = a * Math.Pow(interval.Min, 2);
            double y1 = a * Math.Pow(interval.Max, 2);

            Point3d p0 = new Point3d(interval.Min, y0, 0);
            Point3d p1 = new Point3d(interval.Max, y1, 0);

            Section = NurbsCurve.CreateParabolaFromFocus(Focus, p0, p1);
        }
    }
}
