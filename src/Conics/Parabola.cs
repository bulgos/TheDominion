using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Linq;

namespace the_Dominion.Conics
{
    public class Parabola : ConicSection
    {
        public Parabola()
            : this(1, Interval.Unset) { }

        public Parabola(double a, Interval domain)
            : this(a, 0, 0, Plane.Unset, domain) { }

        public Parabola(double a, double b, double c)
            : this(a, b, c, Plane.Unset, Interval.Unset) { }

        public Parabola(double a, double b, double c, Plane plane, Interval domain)
            : base(plane)
        {
            A = a;
            B = b;
            C = c;

            if (domain != Interval.Unset)
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

            //p1.Transform(Transform);
            //p2.Transform(Transform);
            //p3.Transform(Transform);

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

        public Interval Domain { get; } = new Interval(-10, 10);

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

        public Point3d ComputeParabolaVertex()
        {
            double x = -B / (2 * A);

            return ComputeParabolaPoint(x);
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

        protected override void ComputeFocus()
        {
            Focus = VertexPlane.Origin + new Point3d(0, A / 4, 0);
        }

        public override void TransformShape()
        {
            base.TransformShape();

            Plane vertexPlane = VertexPlane;
            vertexPlane.Transform(Transform);
            VertexPlane = vertexPlane;
        }

        public override ConicSection Duplicate()
        {
            return new Parabola(this);
        }

        public static Parabola[] ComputeParabolasThroughFourPoints(Point3d p1, Point3d p2, Point3d p3, Point3d p4)
        {
            // https://www.mathpages.com/home/kmath037/kmath037.htm
            // https://math.stackexchange.com/a/3224627

            var ray1 = new Line(p1, p3);
            var ray2 = new Line(p2, p3);
            var ray4 = new Line(p4, p3);

            var ray1Flipped = new Line(p3, p1);
            var ray2Flipped = new Line(p3, p2);
            var ray4Flipped = new Line(p3, p4);

            var t1 = AngleBetweenLines(ray1Flipped, ray2Flipped);
            var t2 = AngleBetweenLines(ray2Flipped, ray4Flipped);

            var s1 = Math.Sin(t1);
            var s2 = Math.Sin(t2);
            var c1 = Math.Sin(t1);
            var c2 = Math.Sin(t2);

            var a = s1 * s2 * (s2 * ray2.Length - s1 * ray1.Length);
            var b = 2 * s2 * s1 * (c1 * ray1.Length - c2 * ray2.Length);
            var c = s2 * c1 * (ray4.Length - c1 * ray1.Length) - s1 * c2 * (ray4.Length - c2 * ray2.Length);

            var roots = ComputeQuadraticRoots(a, b, c);

            var basePlane = Plane.WorldXY;
            //basePlane.Rotate(t2, basePlane.ZAxis);

            Parabola parabola = new Parabola(basePlane, p1, p2, p4);

            return new Parabola[] { parabola, parabola };
        }

        private static double AngleBetweenLines(Line l1, Line l2)
        {
            return AngleBetweenVectors(l1.Direction, l2.Direction);
        }

        private static double AngleBetweenVectors(Vector3d v1, Vector3d v2)
        {
            double t1 = VectorAngle(v1);
            double t2 = VectorAngle(v2);

            return t2 - t1;
        }

        public static double VectorAngle(Vector3d vector)
        {
            return Math.Atan2(vector.Y, vector.X);
        }

        private static Tuple<double, double> ComputeQuadraticRoots(double a, double b, double c)
        {
            var discriminant = b * b - 4 * a * c;

            var root1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
            var root2 = (-b - Math.Sqrt(discriminant)) / (2 * a);

            return new Tuple<double, double>(root1, root2);
        }

        public void ConstructParabolaFromFocus()
        {
            throw new NotImplementedException();
        }
    }
}
