using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Rhino.Collections;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;
using the_Dominion.Utility;

namespace the_Dominion.Conics
{
    public class Parabola : ConicSection
    {
        private double _parabolaDiscriminant = double.NaN;
        private Point3d[] _roots = null;

        public Parabola(ConicSection conicSection)
            : base(conicSection)
        {
            if (ConicSectionType != ConicSectionType.Parabola)
                throw new ArgumentException("Conic does not represent a Parabola");

            ConicSection worldAlignedConic = conicSection.WorldAlignedConic;

            Transform(InverseTransformMatrix, false, false, true);
            ConstructParabola();
        }

        public Parabola(double a, Interval domain)
            : this(a, 0, 0, Plane.Unset, domain) { }

        public Parabola(double a, double b, double c, Plane plane, Interval domain)
            : base(plane, a, 0, 0, b, -1, c)
        {
            if (domain != Interval.Unset)
                Domain = domain;

            Initialise();
        }

        public Parabola(Parabola parabola)
            : base(parabola)
        {
            Section = parabola.Section;

            A = parabola.A;
            D = parabola.D;
            F = parabola.F;
            Domain = parabola.Domain;
            VertexPlane = parabola.VertexPlane;
        }

        public Interval Domain { get; } = new Interval(-10, 10);

        public Plane VertexPlane { get; private set; }

        public Point3d[] Roots
        {
            get
            {
                if (_roots == null)
                {
                    //ComputeQuadraticRoots();
                    _roots = new Point3d[0];
                }

                return _roots;
            }
            private set => _roots = value;
        }

        public double ParabolaDiscriminant
        {
            get
            {
                if (double.IsNaN(_parabolaDiscriminant))
                    ComputeParabolaDiscriminant();

                return _parabolaDiscriminant;
            }
            set => _parabolaDiscriminant = value;
        }

        public override bool UsesFlippedAxes => AxisB != 0;

        public ParabolaShape ParabolaShape => GetParabolaShape();

        public static Parabola From3Points(Point3d p1, Point3d p2, Point3d p3, Plane plane, Interval domain)
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

        public static Parabola[] From4Points(IEnumerable<Point3d> pts)
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
            Parabola parabola1 = From3Points(pList[0], pList[1], pList[3], plane1, domain1);
            Parabola parabola2 = From3Points(pList[0], pList[1], pList[3], plane2, domain2);

            return new Parabola[] { parabola1, parabola2 };
        }

        public void Initialise()
        {
            ConstructParabola();
            ComputeVertexPlane();
            ComputeFoci();
            Transform(TransformMatrix, true, true, false);
        }

        public void ConstructParabola()
        {
            Point3d p0;
            Point3d p2;

            if (!UsesFlippedAxes)
            {
                p0 = WorldAlignedConic.ComputePointAtX(Domain.Min)[0];
                p2 = WorldAlignedConic.ComputePointAtX(Domain.Max)[0];
            }
            else
            {
                p0 = WorldAlignedConic.ComputePointAtY(Domain.Min)[0];
                p2 = WorldAlignedConic.ComputePointAtY(Domain.Max)[0];
            }

            Line tangent1 = WorldAlignedConic.ComputeTangent(p0);
            Line tangent2 = WorldAlignedConic.ComputeTangent(p2);

            Point3d p1 = Geometry.ComputeLineIntersection(tangent1, tangent2);

            Point3d[] points = { tangent1.From, p1, tangent2.From };

            Section = Curve.CreateControlPointCurve(points, 2) as NurbsCurve;
        }

        public void ConstructParabolaFromFocus()
        {
            throw new NotImplementedException();
        }

        public override double ComputeDerivative(Point3d pt)
        {
            return !UsesFlippedAxes
                ? 2 * A * pt.X + D
                : (2 * C * pt.Y + E);
        }

        public override Line ComputeTangent(Point3d pt)
        {
            double derivative = ComputeDerivative(pt);

            Vector3d direction = !UsesFlippedAxes
                ? new Vector3d(1, derivative, 0)
                : new Vector3d(derivative, 1, 0);
            direction.Unitize();

            return new Line(pt, direction);
        }

        protected override void ComputeFoci()
        {
            double vertexParameter;

            if (UsesFlippedAxes)
                vertexParameter = Math.Abs(AxisB);
            else
                vertexParameter = Math.Abs(AxisA);

            Focus1 = VertexPlane.Origin + (VertexPlane.YAxis * vertexParameter / 4);
        }

        public void ComputeParabolaPoint(ref Point3d pt)
        {
            if (UsesFlippedAxes)
                pt.X = WorldAlignedConic.C * pt.Y * pt.Y + WorldAlignedConic.E * pt.Y + WorldAlignedConic.F;
            else
                pt.Y = WorldAlignedConic.A * pt.X * pt.X + WorldAlignedConic.D * pt.X + WorldAlignedConic.F;
        }

        public Point3d ComputeParabolaVertex()
        {
            Point3d pt = new Point3d();

            if (UsesFlippedAxes)
                pt.Y = -WorldAlignedConic.E / (2 * WorldAlignedConic.C);
            else
                pt.X = -WorldAlignedConic.D / (2 * WorldAlignedConic.A);

            ComputeParabolaPoint(ref pt);

            return pt;
        }

        private void ComputeVertexPlane()
        {
            var vertex = ComputeParabolaVertex();

            double angle = Math.PI / 2;

            switch (ParabolaShape)
            {
                case ParabolaShape.NegativeX:
                    break;
                case ParabolaShape.NegativeY:
                    angle *= 2;
                    break;
                case ParabolaShape.PositiveX:
                    angle *= 3;
                    break;
                case ParabolaShape.PositiveY:
                    angle = 0;
                    break;
            }

            var vertexPlane = Plane.WorldXY;
            vertexPlane.Rotate(angle, vertexPlane.ZAxis);
            vertexPlane.Origin = vertex;

            VertexPlane = vertexPlane;
        }

        protected override void TransformProperties(Transform xform)
        {
            base.TransformProperties(xform);

            Plane vertexPlane = VertexPlane;
            vertexPlane.Transform(TransformMatrix);
            VertexPlane = vertexPlane;
        }

        private static Point3dList GetTransformedPoints(Point3dList pts)
        {
            Vector3d translationVector = Point3d.Origin - pts[2];
            Transform translation = Rhino.Geometry.Transform.Translation(translationVector);

            double rotationAngle = -(pts[3] - pts[2]).VectorAngle();
            Transform rotation = Rhino.Geometry.Transform.Rotation(rotationAngle, Point3d.Origin);

            double transformScale = 1 / (pts[3] - pts[2]).Length;
            Transform scale = Rhino.Geometry.Transform.Scale(Point3d.Origin, transformScale);

            var ptsTransformed = new Point3dList(pts);
            ptsTransformed.Transform(translation);
            ptsTransformed.Transform(rotation);
            ptsTransformed.Transform(scale);

            return ptsTransformed;
        }

        private void ComputeQuadraticRoots()
        {
            double[] rootParameters = Geometry.ComputeQuadraticRoots(A, D, F);

            var roots = new Point3d[rootParameters.Length];

            for (int i = 0; i < rootParameters.Length; i++)
            {
                Point3d rootPt = new Point3d(rootParameters[i], 0, 0);
                ComputeParabolaPoint(ref rootPt);
                rootPt.Transform(TransformMatrix);

                roots[i] = rootPt;
            }

            Roots = roots;
        }

        private void ComputeParabolaDiscriminant()
        {
            ParabolaDiscriminant = Geometry.ComputeDiscriminant(A, D, F);
        }

        private ParabolaShape GetParabolaShape()
        {
            if (AxisB == 0)
            {
                if (WorldAlignedConic.A * WorldAlignedConic.E < 0)
                    return ParabolaShape.PositiveY;
                else
                    return ParabolaShape.NegativeY;
            }
            else
            {
                if (WorldAlignedConic.C * WorldAlignedConic.D < 0)
                    return ParabolaShape.PositiveX;
                else
                    return ParabolaShape.NegativeX;
            }
        }

        public override ConicSection Duplicate()
        {
            return new Parabola(this);
        }
    }

    public enum ParabolaShape
    {
        NegativeX,
        NegativeY,
        PositiveX,
        PositiveY
    }
}