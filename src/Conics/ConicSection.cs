using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using the_Dominion.Utility;

namespace the_Dominion.Conics
{
    public class ConicSection
    {
        private Transform _inverseTransform = Transform.Unset;
        private double _conicDiscriminant = double.NaN;

        public ConicSection(double a, double b, double c, double d, double e, double f)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;

            GetConicTransform();
        }

        public ConicSection(IEnumerable<Point3d> points)
        {
            From5Points(points);
        }

        protected ConicSection()
            : this(Plane.WorldXY) { }

        protected ConicSection(Plane plane)
        {
            if (plane == Plane.Unset || plane == Plane.WorldXY)
                return;

            BasePlane = plane;
            GetTransform(plane);
        }

        public ConicSection(ConicSection conicSection)
            : this(conicSection.BasePlane)
        {
            Section = conicSection.Section;
            Focus1 = conicSection.Focus1;
            Focus2 = conicSection.Focus2;
            A = conicSection.A;
            B = conicSection.B;
            C = conicSection.C;
            D = conicSection.D;
            E = conicSection.E;
            F = conicSection.F;
            ConicDiscriminant = conicSection.ConicDiscriminant;
            Transform = conicSection.Transform;
        }

        public Plane BasePlane { get; } = Plane.WorldXY;

        public NurbsCurve Section { get; protected set; }

        public Point3d Focus1 { get; protected set; } = Point3d.Unset;

        public Point3d Focus2 { get; protected set; } = Point3d.Unset;

        public double A { get; protected set; }

        public double B { get; protected set; }

        public double C { get; protected set; }

        public double D { get; protected set; }

        public double E { get; protected set; }

        public double F { get; protected set; }

        public double ConicDiscriminant
        {
            get
            {
                if (double.IsNaN(_conicDiscriminant))
                    ComputeConicDiscriminant();

                return _conicDiscriminant;
            }
            set => _conicDiscriminant = value;
        }

        public ConicSectionType ConicSectionType => GetConicType();

        public Transform Transform { get; private set; } = Transform.Identity;

        public Transform InverseTransform
        {
            get
            {
                if (_inverseTransform == Transform.Unset)
                {
                    Transform.TryGetInverse(out _inverseTransform);
                }

                return _inverseTransform;
            }
        }

        public BoundingBox BoundingBox =>
            IsValid
            ? Section.GetBoundingBox(Transform)
            : BoundingBox.Empty;

        public virtual bool IsValid => Section != null;

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

            ConicSection conicSection = new ConicSection(a, b, c, d, e, f);


            switch (conicSection.ConicSectionType)
            {
                case ConicSectionType.Ellipse:
                    return new Ellipse(conicSection, pts[0], pts[1]);
                case ConicSectionType.Hyperbola:
                    return new Hyperbola(conicSection, pts[0], pts[1]);
                case ConicSectionType.Parabola:
                    return new Parabola(conicSection);
                default:
                    return conicSection;
            }
        }

        public Point3d[] ComputePointAtX(double x)
        {
            double a = C;
            double b = B * x + E;
            double c = A * x * x + D * x + F;

            return a == 0
                ? new Point3d[] { new Point3d(x, -c / b, 0) }
                : Geometry.ComputeQuadraticRoots(a, b, c).Select(y => new Point3d(x, y, 0)).ToArray();
        }

        public Point3d[] ComputePointAtY(double y)
        {
            double a = A;
            double b = B * y + D;
            double c = C * y * y + E * y + F;

            return a == 0
                ? new Point3d[] { new Point3d(-c / b, y, 0) }
                : Geometry.ComputeQuadraticRoots(a, b, c).Select(x => new Point3d(x, y, 0)).ToArray();
        }

        private ConicSectionType GetConicType()
        {
            if (ConicDiscriminant < 0)
            {
                return ConicSectionType.Ellipse;
            }

            if (ConicDiscriminant > 0)
            {
                return ConicSectionType.Hyperbola;
            }

            return ConicSectionType.Parabola;
        }

        protected virtual void ComputeFocus()
        {

        }

        private void ComputeConicDiscriminant()
        {
            ConicDiscriminant = Geometry.ComputeDiscriminant(A, B, C);
        }

        public double ComputeConicRotation()
        {
            return Geometry.ACot((A - C) / B) / 2;
        }

        public Vector3d ComputeConicTranslation()
        {
            double x = (2 * C * D - B * E) / ConicDiscriminant;
            double y = (2 * A * E - B * D) / ConicDiscriminant;

            return new Vector3d(x, y, 0);
        }

        public void TransformToStandardConic()
        {
            TranslateConic();
            RotateConic();

            // normalise parameters
            double factor = -1 / F;

            A *= factor;
            C *= factor;
            F = -1;
        }

        public void TranslateConic()
        {
            Vector3d vector = ComputeConicTranslation();

            TranslateConic(vector);
        }

        public void TranslateConic(Vector3d vector)
        {
            double h = vector.X;
            double k = vector.Y;

            double d = 2 * A * h + B * k + D;
            double e = B * h + 2 * C * k + E;
            double f = A * h * h + B * h * k + C * k * k + D * h + E * k + F;

            // A, B, C remain unchanged
            D = d;
            E = e;
            F = f;
        }

        public void RotateConic()
        {
            double angle = ComputeConicRotation();

            RotateConic(angle);
        }

        public void RotateConic(double angle)
        {
            // https://en.wikipedia.org/wiki/Rotation_of_axes

            double cost = Math.Cos(angle);
            double sint = Math.Sin(angle);
            double cos2t = Math.Pow(cost, 2);
            double sin2t = Math.Pow(sint, 2);

            double a = A * cos2t + B * sint * cost + C * sin2t;
            double b = 2 * (C - A) * sint * cost + B * (cos2t - sin2t);
            double c = A * sin2t - B * sint * cost + C * cos2t;
            double d = D * cost + E * sint;
            double e = -D * sint + E * cost;

            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            // F remains unchanged
        }

        public void GetConicTransform()
        {
            double rotation = ComputeConicRotation();
            Vector3d translation = ComputeConicTranslation();

            Transform rotate = Transform.Rotation(rotation, Point3d.Origin);
            Transform translate = Transform.Translation(translation);

            Transform = translate * rotate;
        }

        private void GetTransform(Plane targetPlane)
        {
            Transform = Transform.PlaneToPlane(Plane.WorldXY, targetPlane);
        }

        public virtual void TransformShape()
        {
            TransformShape(Transform);
        }

        public virtual void TransformShape(Transform xform)
        {
            if (!IsValid || xform == Transform.Identity)
                return;

            Section.Transform(xform);

            Point3d focus = Focus1;
            focus.Transform(xform);
            Focus1 = focus;
        }

        #region GH_GeometricGoo tools

        public virtual ConicSection Duplicate()
        {
            return new ConicSection(this);
        }

        public BoundingBox GetBoundingBox(Transform xform)
        {
            if (!IsValid)
                return BoundingBox.Empty;

            return Section.GetBoundingBox(xform);
        }

        public bool Morph(SpaceMorph xmorph)
        {
            if (!IsValid)
                return false;

            return xmorph.Morph(Section);
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        #endregion
    }
}
