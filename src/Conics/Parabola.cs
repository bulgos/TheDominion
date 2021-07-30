using Rhino.Collections;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using the_Dominion.Utility;

namespace the_Dominion.Conics
{
    public class Parabola : ConicSection
    {
        private Interval _domain = new Interval(-10, 10);

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
        }

        public Parabola(Point3d p1, Point3d p2, Point3d p3, Plane plane)
            : this(p1, p2, p3, plane, Interval.Unset) { }

        public Parabola(Point3d p1, Point3d p2, Point3d p3, Plane plane, Interval domain)
            : base(plane)
        {
            p1.Transform(InverseTransform);
            p2.Transform(InverseTransform);
            p3.Transform(InverseTransform);

            Tuple<double, double, double> quadratic = ComputeQuadraticParametersFrom3Points(p1, p2, p3);

            A = quadratic.Item1;
            B = quadratic.Item2;
            C = quadratic.Item3;

            Point3d[] points = { p1, p2, p3 };

            // compute bounds if domain is unset
            Domain = domain == Interval.Unset
                ? points.ComputeBounds()
                : domain;

            ConstructParabola();
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

        public Interval Domain
        {
            get => _domain;
            set
            {
                _domain = value;
                ConstructParabola();
            }
        }

        public Plane VertexPlane { get; private set; }

        public Tuple<double, Point3d>[] Roots => ComputeQuadraticRoots();

        public void ConstructParabola()
        {
            Point3d p0 = ComputeTangentIntersections(Domain.Min, Domain.Max, out Line tangent1, out Line tangent2);

            Point3d[] points = { tangent1.From, p0, tangent2.From };

            Section = Curve.CreateControlPointCurve(points, 2) as NurbsCurve;

            ComputeVertexPlane();
            ComputeFocus();
            TransformShape();
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
            Focus1 = VertexPlane.Origin + new Point3d(0, A / 4, 0);
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

            // this method requires a transformation such that
            // p3 is at the origin (0, 0, 0)
            // and p4 is at (1, 0, 0)
            // this unitizes the problem such that we can solve across p1 and p2

            // create new points which meet the translation, rotation and scale requirements to solve
            var transformedPoints = GetTransformedPoints(p1, p2, p3, p4);
            Point3d p1xForm = transformedPoints.Item1;
            Point3d p2xForm = transformedPoints.Item2;

            // we calculate the angle quadratic of the form A*tan(t)^2 + B*Tan(t) + C = 0
            double a = p2xForm.Y - p1xForm.Y;
            double b = 2 * (p2xForm.X - p1xForm.X);
            double c = p2xForm.X * (p2xForm.X - 1) / p2xForm.Y - p1xForm.X * (p1xForm.X - 1) / p1xForm.Y;

            // the angle of p4-p3 gives us the transformation.
            double rotation = (p4 - p3).VectorAngle();

            // calculating the roots gives us the solution tan(t) = root1, root2
            // sp we take the inverse Tan to find the angle at which valid parabolae will form
            var roots = ComputeQuadraticRoots(a, b, c);

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
            Point3d[] points = { p1, p2, p3, p4 };

            Interval domain1 = points.ComputeTransformedBoundsInPlane(plane1, Plane.WorldXY);
            Interval domain2 = points.ComputeTransformedBoundsInPlane(plane2, Plane.WorldXY);

            // construct the parabolas from three of the points and the calculated plane
            Parabola parabola1 = new Parabola(p1, p2, p4, plane1);
            Parabola parabola2 = new Parabola(p1, p2, p4, plane2);

            // set the domain
            parabola1.Domain = domain1;
            parabola2.Domain = domain2;

            return new Parabola[] { parabola1, parabola2 };
        }

        private static Tuple<Point3d, Point3d, Point3d, Point3d> GetTransformedPoints(Point3d p1, Point3d p2, Point3d p3, Point3d p4)
        {
            Vector3d translationVector = Point3d.Origin - p3;
            Transform translation = Transform.Translation(translationVector);

            double rotationAngle = -(p4 - p3).VectorAngle();
            Transform rotation = Transform.Rotation(rotationAngle, Point3d.Origin);

            double transformScale = 1 / (p4 - p3).Length;
            Transform scale = Transform.Scale(Point3d.Origin, transformScale);

            Point3dList points = new Point3dList() { new Point3d(p1), new Point3d(p2), new Point3d(p3), new Point3d(p4) };

            for (int i = 0; i < points.Count; i++)
            {
                var pt = points[i];

                pt.Transform(translation);
                pt.Transform(rotation);
                pt.Transform(scale);

                points[i] = pt;
            }

            return new Tuple<Point3d, Point3d, Point3d, Point3d>(points[0], points[1], points[2], points[3]);
        }

        private Tuple<double, Point3d>[] ComputeQuadraticRoots()
        {
            double[] rootParameters = ComputeQuadraticRoots(A, B, C);

            var roots = new Tuple<double, Point3d>[rootParameters.Length];

            for (int i = 0; i < rootParameters.Length; i++)
            {
                var rootPt = ComputeParabolaPoint(rootParameters[i]);
                rootPt.Transform(Transform);

                roots[i] = new Tuple<double, Point3d>(rootParameters[i], rootPt);
            }

            return roots;
        }

        private static double[] ComputeQuadraticRoots(double a, double b, double c)
        {
            double discriminant = Geometry.ComputeDiscriminant(a, b, c);

            if (discriminant < 0)
            {
                return new double[0];
            }

            else if (discriminant > 0)
            {
                double root1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
                double root2 = (-b - Math.Sqrt(discriminant)) / (2 * a);

                return new double[] { root1, root2 };
            }

            else
            {
                return new double[] { -b / (2 * a) };
            }
        }

        public void ConstructParabolaFromFocus()
        {
            throw new NotImplementedException();
        }
    }
}
