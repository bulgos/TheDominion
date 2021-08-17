using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Rhino.Collections;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using the_Dominion.Utility;

namespace the_Dominion.Conics
{
    public class ConicSection
    {
        private Transform _inverseTransformMatrix = Rhino.Geometry.Transform.Unset;
        private double _conicDiscriminant = double.NaN;

        protected ConicSection(double a, double b, double c, double d, double e, double f)
            : this(Plane.Unset, a, b, c, d, e, f) { }

        protected ConicSection(Plane plane, double a, double b, double c, double d, double e, double f)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;

            // transform equation parameters if a valid 2d plane is supplied
            if (plane != Plane.Unset && plane != Plane.WorldXY)
            {
                if (Math.Abs(1 - Vector3d.Multiply(plane.ZAxis, Vector3d.ZAxis)) < Rhino.RhinoMath.ZeroTolerance)
                {
                    var xform = Geometry.WorldXYToPlaneTransform(plane);
                    TransformEquation(xform);
                }
                else
                {
                    TransformMatrix = Geometry.WorldXYToPlaneTransform(plane);
                }
            }

            GetWorldAlignedConic();
            ComputeEquationTransform();
            ComputeAxes();
            plane = Plane.WorldXY;
            plane.Transform(TransformMatrix);

            BasePlane = plane;
        }

        public ConicSection(ConicSection conicSection)
        {
            if (conicSection.IsValid)
            {
                foreach (var section in conicSection.Section)
                {
                    Section.Add((section.Duplicate() as Curve).ToNurbsCurve());
                }
            }

            BasePlane = conicSection.BasePlane;
            Focus1 = conicSection.Focus1;
            Focus2 = conicSection.Focus2;
            A = conicSection.A;
            B = conicSection.B;
            C = conicSection.C;
            D = conicSection.D;
            E = conicSection.E;
            F = conicSection.F;
            ConicDiscriminant = conicSection.ConicDiscriminant;
            TransformMatrix = conicSection.TransformMatrix;
            WorldAlignedConic = conicSection.WorldAlignedConic;

            AxisA = conicSection.AxisA;
            AxisB = conicSection.AxisB;
        }

        public double A { get; protected set; }

        public double B { get; protected set; }

        public double C { get; protected set; }

        public double D { get; protected set; }

        public double E { get; protected set; }

        public double F { get; protected set; }

        public double AxisA { get; protected set; }

        public double AxisB { get; protected set; }

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

        public virtual bool IsValid => Section.Count > 0;

        public bool IsWorldAligned => B == 0;

        public virtual bool UsesFlippedAxes => false;

        public ConicSectionType ConicSectionType => GetConicType();

        public Plane BasePlane { get; }

        public Point3d Focus1 { get; protected set; } = Point3d.Unset;

        public Point3d Focus2 { get; protected set; } = Point3d.Unset;

        public Transform InverseTransformMatrix
        {
            get
            {
                if (_inverseTransformMatrix == Rhino.Geometry.Transform.Unset)
                {
                    TransformMatrix.TryGetInverse(out _inverseTransformMatrix);
                }

                return _inverseTransformMatrix;
            }
        }

        public Transform TransformMatrix { get; private set; } = Rhino.Geometry.Transform.Identity;

        public BoundingBox BoundingBox =>
            IsValid
            ? GetBoundingBox(TransformMatrix)
            : BoundingBox.Empty;

        public CurveList Section { get; protected set; } = new CurveList();

        public ConicSection WorldAlignedConic { get; private set; }

        public static ConicSection From5PointsAndTangents(IEnumerable<Point3d> points, IEnumerable<Line> lines)
        {
            // bugs
            List<Point3d> pts = points.ToList();
            List<Line> lns = lines.ToList();

            if (pts.Count + lns.Count != 5)
                throw new ArgumentException("Incorrect number of points and tangents specified");

            foreach (Line ln in lns)
            {
                pts.Add(ln.ComputeDual());
            }

            return From5Points(pts);
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
            ConicSection conicSection = new ConicSection(a, b, c, d, e, f);

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

        public void ComputeAxes()
        {
            // bug: circular reference exists here, so WorldAlignedConic needs to be recomputed after Axes
            if (Math.Abs(WorldAlignedConic.A) > Rhino.RhinoMath.ZeroTolerance)
                AxisA = Math.Pow(Math.Abs(WorldAlignedConic.A), -1.0 / 2.0);
            if (Math.Abs(WorldAlignedConic.C) > Rhino.RhinoMath.ZeroTolerance)
                AxisB = Math.Pow(Math.Abs(WorldAlignedConic.C), -1.0 / 2.0);

            if (WorldAlignedConic.A < 0)
                AxisA *= -1;
            if (WorldAlignedConic.C < 0)
                AxisB *= -1;

            GetWorldAlignedConic();
        }

        public virtual Line ComputeTangent(Point3d pt)
        {
            double derivative = ComputeDerivative(pt);

            Vector3d direction = new Vector3d(1, derivative, 0);
            direction.Unitize();

            return new Line(pt, direction);
        }

        public Point3d[] ComputePointAtX(double x)
        {
            if (double.IsNaN(x))
                return new Point3d[0];

            double a = C;
            double b = B * x + E;
            double c = A * x * x + D * x + F;

            return a == 0
                ? new Point3d[] { new Point3d(x, -c / b, 0) }
                : Geometry.ComputeQuadraticRoots(a, b, c).Select(y => new Point3d(x, y, 0)).ToArray();
        }

        public Point3d[] ComputePointAtY(double y)
        {
            if (double.IsNaN(y))
                return new Point3d[0];

            double a = A;
            double b = B * y + D;
            double c = C * y * y + E * y + F;

            return Math.Abs(a) < Rhino.RhinoMath.ZeroTolerance
                ? new Point3d[] { new Point3d(-c / b, y, 0) }
                : Geometry.ComputeQuadraticRoots(a, b, c).Select(x => new Point3d(x, y, 0)).ToArray();
        }

        private ConicSectionType GetConicType()
        {
            if (ConicDiscriminant > -Rhino.RhinoMath.ZeroTolerance && ConicDiscriminant < Rhino.RhinoMath.ZeroTolerance)
                return ConicSectionType.Parabola;

            if (ConicDiscriminant < 0)
            {
                if (A == C)
                    return ConicSectionType.Circle;

                return ConicSectionType.Ellipse;
            }

            if (ConicDiscriminant > 0)
            {
                return ConicSectionType.Hyperbola;
            }

            return ConicSectionType.Unknown;
        }

        public virtual double ComputeDerivative(Point3d pt)
        {
            throw new ArgumentException("Can only Compute Derivative for derived classes, not for base class.");
        }

        protected virtual void ComputeFoci()
        {
            throw new ArgumentException("Can only Compute Focus for derived classes, not for base class.");
        }

        private void ComputeConicDiscriminant()
        {
            ConicDiscriminant = Geometry.ComputeDiscriminant(A, B, C);
        }

        #region transformation methods

        public void ComputeBasePlaneTransform()
        {
            TransformMatrix = Geometry.WorldXYToPlaneTransform(BasePlane);
        }

        public void ComputeEquationTransform()
        {
            // https://math.stackexchange.com/questions/982908/deal-with-non-standard-form-of-conic
            Vector3d translation = ComputeEquationTranslation();
            double rotation = ComputeEquationRotation();

            Transform rotate = Rhino.Geometry.Transform.Rotation(rotation, Point3d.Origin);
            Transform translate = Rhino.Geometry.Transform.Translation(translation);

            TransformMatrix = translate * rotate;
        }

        private double ComputeEquationRotation()
        {
            if (ConicSectionType == ConicSectionType.Parabola)
            {
                return B == 0
                    ? 0
                    : Geometry.ACot((A - C) / B) / 2;
            }
            else
            {
                return B == 0
                    ? 0
                    : Geometry.ACotContinuous((A - C) / B) / 2;
            }
        }

        public Vector3d ComputeEquationTranslation()
        {
            // special case required for Parabola because Discriminant == 0
            // and will produce a divide by zero error
            if (ConicSectionType == ConicSectionType.Parabola)
            {
                //return new Vector3d(D / (2 * A), F - (D * D) / (4 * A), 0);
                return Vector3d.Zero;
            }

            double x = (2 * C * D - B * E) / ConicDiscriminant;
            double y = (2 * A * E - B * D) / ConicDiscriminant;

            return new Vector3d(x, y, 0);
        }

        private void EliminateTransformationFromEquation()
        {
            EliminateTransformationFromEquation(out _, out _);
        }

        private void EliminateTransformationFromEquation(out Vector3d translation, out double rotation)
        {
            // https://math.stackexchange.com/questions/982908/deal-with-non-standard-form-of-conic

            translation = ComputeEquationTranslation();
            rotation = ComputeEquationRotation();

            TranslateEquation(-translation);
            RotateEquation(-rotation);

            Unitize();
        }

        private void Unitize()
        {
            if (Math.Abs(A) < Rhino.RhinoMath.ZeroTolerance)
                A = 0;
            if (Math.Abs(B) < Rhino.RhinoMath.ZeroTolerance)
                B = 0;
            if (Math.Abs(C) < Rhino.RhinoMath.ZeroTolerance)
                C = 0;
            if (Math.Abs(D) < Rhino.RhinoMath.ZeroTolerance)
                D = 0;
            if (Math.Abs(E) < Rhino.RhinoMath.ZeroTolerance)
                E = 0;
            if (Math.Abs(F) < Rhino.RhinoMath.ZeroTolerance)
                F = 0;

            if (!IsWorldAligned)
                return;

            double factor;

            // normalise parameters
            if (E != 0 && A != 0)
            {
                factor = -1 / E;
                A *= factor;
                D *= factor;
                E = -1;
                F *= factor;
            }
            if (D != 0 && C != 0)
            {
                factor = -1 / D;
                C *= factor;
                D = -1;
                E *= factor;
                F *= factor;
            }
            if (A != 0 && C != 0 && F != 0)
            {
                factor = -1 / F;
                A *= factor;
                C *= factor;
                F = -1;
            }
        }

        private void TranslateEquation(Vector3d vector)
        {
            vector *= -1;

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

        private void RotateEquation(double angle)
        {
            // https://en.wikipedia.org/wiki/Rotation_of_axes

            angle *= -1;

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

        public void Transform(Transform xform, bool transformShape = true, bool transformProperties = true, bool transformEquation = false)
        {
            if (xform == Rhino.Geometry.Transform.Identity)
                return;

            if (transformShape)
                TransformShape(xform);

            if (transformProperties)
                TransformProperties(xform);

            if (transformEquation)
                TransformEquation(xform);
        }

        private void TransformEquation(Vector3d translation, double rotation)
        {
            RotateEquation(rotation);
            TranslateEquation(translation);
        }

        private void TransformEquation(Transform xform)
        {
            // firstly remove transform, but store transforms to achieve previous equation
            EliminateTransformationFromEquation(out Vector3d eqTranslation, out double eqRotation);

            if (xform.IsAffine)
            {
                xform.DecomposeAffine(out Vector3d translation, out Transform rotation, out Transform ortho, out Vector3d diagonal);
                rotation.GetYawPitchRoll(out double yaw, out double _, out double _);

                translation += eqTranslation;
                yaw += eqRotation;

                TransformEquation(translation, yaw);
            }
        }

        private void TransformShape(Transform xform)
        {
            if (!IsValid)
                return;

            Section.Transform(xform);
        }

        protected virtual void TransformProperties(Transform xform)
        {
            Point3d focus1 = Focus1;
            Point3d focus2 = Focus2;
            focus1.Transform(xform);
            focus2.Transform(xform);
            Focus1 = focus1;
            Focus2 = focus2;
        }

        private void GetWorldAlignedConic()
        {
            ConicSection worldAlignedConic = Duplicate();
            worldAlignedConic.EliminateTransformationFromEquation();

            WorldAlignedConic = worldAlignedConic;
        }

        #endregion

        public string FormatConicEquation()
        {
            double[] terms = new[] { A, B, C, D, E, F };
            char term = 'A';

            string formatString = "Conic:";

            for (int i = 0; i < 6; i++)
            {
                formatString += $"\n{term} = {terms[i].ToString("F8")}";
                term++;
            }

            return formatString;
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

            BoundingBox bbox = BoundingBox.Unset;

            foreach (var section in Section)
            {
                if (!bbox.IsValid)
                {
                    bbox = section.GetBoundingBox(xform);
                    continue;
                }

                bbox.Union(section.GetBoundingBox(xform));
            }

            return bbox;
        }

        public bool Morph(SpaceMorph xmorph)
        {
            //if (!IsValid)
            //    return false;

            //return xmorph.Morph(Section);
            return false;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        #endregion
    }
}