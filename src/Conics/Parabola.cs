using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Linq;

namespace the_Dominion.Conics
{
    public class Parabola : ConicSection
    {
        public Parabola()
            : this(1, new Interval(-10, 10)) { }

        public Parabola(double a, Interval domain)
            : this(Plane.WorldXY, a, 0, 0, domain) { }

        public Parabola(Plane plane, double a, double b, double c, Interval domain)
            : base(plane)
        {
            A = a;
            B = b;
            C = c;
            Domain = domain;

            ConstructParabola();
            TransformShape();
        }

        public Parabola(Plane plane, Point3d p1, Point3d p2, Point3d p3)
            : base(plane)
        {
            p1.Transform(InverseTransform);
            p2.Transform(InverseTransform);
            p3.Transform(InverseTransform);

            Tuple<double, double, double> quadratic = ComputeQuadraticParametersFrom3Points(p1, p2, p3);

            A = quadratic.Item1;
            B = quadratic.Item2;
            C = quadratic.Item3;

            double[] xValues = { p1.X, p2.X, p3.X };

            Domain = new Interval(xValues.Min(), xValues.Max());

            ConstructParabola();
            TransformShape();
        }

        public Parabola(Parabola parabola)
            : base(parabola)
        {
            Section = parabola.Section;

            A = parabola.A;
            B = parabola.B;
            C = parabola.C;
            Domain = parabola.Domain;
            VertexPlane = parabola.VertexPlane;
        }

        public double A { get; }

        public double B { get; }

        public double C { get; }

        public Interval Domain { get; } = new Interval(-1, 1);

        public Plane VertexPlane { get; private set; }

        public void ConstructParabola()
        {
            Point3d p0 = ComputeTangentIntersections(Domain.Min, Domain.Max, out Line tangent1, out Line tangent2);

            Point3d[] points = { tangent1.From, p0, tangent2.From };

            Section = Curve.CreateControlPointCurve(points, 2) as NurbsCurve;

            ComputeVertexPlane();
            ComputeFocus();
        }

        private Tuple<double, double, double> ComputeQuadraticParametersFrom3Points(Point3d p1, Point3d p2, Point3d p3)
        {
            /// http://stackoverflow.com/questions/717762/how-to-calculate-the-vertex-of-a-parabola-given-three-points

            double denom = (p1.X - p2.X) * (p1.X - p3.X) * (p2.X - p3.X);
            double a = (p3.X * (p2.Y - p1.Y) + p2.X * (p1.Y - p3.Y) + p1.X * (p3.Y - p2.Y)) / denom;
            double b = (p3.X * p3.X * (p1.Y - p2.Y) + p2.X * p2.X * (p3.Y - p1.Y) + p1.X * p1.X * (p2.Y - p3.Y)) / denom;
            double c = (p2.X * p3.X * (p2.X - p3.X) * p1.Y + p3.X * p1.X * (p3.X - p1.X) * p2.Y + p1.X * p2.X * (p1.X - p2.X) * p3.Y) / denom;

            return new Tuple<double, double, double>(a, b, c);
        }

        public Point3d ComputeParabolaVertex()
        {
            double x = -B / (2 * A);

            return ComputeParabolaPoint(x);
        }

        public Point3d ComputeParabolaPoint(double x)
        {
            double y = A * x * x + B * x + C;

            return new Point3d(x, y, 0);
        }

        public Vector3d ComputeParabolaTangentVector(double x)
        {
            double derivative = 2 * A * x + B;

            return new Vector3d(1, derivative, 0);
        }

        public Line ComputeParabolaTangent(double x)
        {
            Point3d pt = ComputeParabolaPoint(x);
            Vector3d tangent = ComputeParabolaTangentVector(x);

            return new Line(pt, tangent);
        }

        public Point3d ComputeTangentIntersections(double x1, double x2, out Line tangent1, out Line tangent2)
        {
            tangent1 = ComputeParabolaTangent(x1);
            tangent2 = ComputeParabolaTangent(x2);

            Intersection.LineLine(tangent1, tangent2, out double param1, out double param2);

            return tangent1.PointAt(param1);
        }

        protected override void ComputeFocus()
        {
            Focus = VertexPlane.Origin + new Point3d(0, A / 4, 0);
        }

        private void ComputeVertexPlane()
        {
            var vertex = ComputeParabolaVertex();

            var vertexPlane = Plane.WorldXY;
            vertexPlane.Origin = vertex;

            if (A < 0)
            {
                vertexPlane.Rotate(Math.PI, vertexPlane.ZAxis);
            }

            VertexPlane = vertexPlane;
        }

        public override void TransformShape()
        {
            base.TransformShape();

            Plane vertexPlane = VertexPlane;
            vertexPlane.Transform(Transform);
            VertexPlane = vertexPlane;
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

        public override ConicSection Duplicate()
        {
            return new Parabola(this);
        }
    }
}
