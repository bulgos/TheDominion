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
        private Transform _inverseTransform = Transform.Unset;
        private double _discriminant = double.NaN;

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
            Transform = GetTransform(plane);
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
            Discriminant = conicSection.Discriminant;
            EquationTransform = conicSection.EquationTransform;
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

        public double Discriminant
        {
            get
            {
                if (double.IsNaN(_discriminant))
                    ComputeDiscriminant();

                return _discriminant;
            }
            set => _discriminant = value;
        }

        public ConicSectionType ConicSectionType => GetConicType();

        public Transform EquationTransform { get; private set; }

        protected Transform Transform { get; } = Transform.Identity;

        protected Transform InverseTransform
        {
            get
            {
                if (_inverseTransform == Transform.Unset)
                {
                    _inverseTransform = Transform.Transpose();
                }

                return _inverseTransform;
            }
        }

        public BoundingBox BoundingBox => 
            IsValid 
            ? Section.GetBoundingBox(Transform) 
            : BoundingBox.Empty;

        public virtual bool IsValid => Section != null;

        public void GetConicTransform()
        {
            double rotation = Geometry.ACot((A - C) / B) / 2;
            Vector3d translation = Vector3d.Zero;

            translation.X = (2 * C * D - B * E) / Discriminant;
            translation.Y = (2 * A * E - B * D) / Discriminant;

            Transform rotate = Transform.Rotation(rotation, Point3d.Origin);
            Transform translate = Transform.Translation(translation);

            EquationTransform = translate * rotate;
        }

        private ConicSectionType GetConicType()
        {
            if (Discriminant < 0)
            {
                return ConicSectionType.Ellipse;
            }

            if (Discriminant > 0)
            {
                return ConicSectionType.Hyperbola;
            }

            return ConicSectionType.Parabola;
        }

        private void ComputeDiscriminant()
        {
            Discriminant = Geometry.ComputeDiscriminant(A, B, C);
        }

        protected virtual void ComputeFocus()
        {

        }

        public static ConicSection From5Points(IEnumerable<Point3d> points)
        {
            // simplest solution we could find
            // https://math.stackexchange.com/a/1987192/951797

            var pts = points.ToArray();

            if (pts.Length != 5)
                throw new ArgumentException("Incorrect number of points specified");

            double[][] matrixValues = new double[5][];
            Vector<double> vector = Vector.Build.Dense(5, -1);

            for (int i = 0; i < pts.Length; i++)
            {
                matrixValues[i] = new[] { pts[i].X * pts[i].X, pts[i].X * pts[i].Y, pts[i].Y * pts[i].Y, pts[i].X, pts[i].Y };
            }

            Matrix<double> matrix = DenseMatrix.OfRowArrays(matrixValues);

            Vector<double> solution = matrix.Solve(vector);

            ConicSection conicSection = new ConicSection();

            conicSection.A = solution[0];
            conicSection.B = solution[1];
            conicSection.C = solution[2];
            conicSection.D = solution[3];
            conicSection.E = solution[4];
            conicSection.F = 1;

            conicSection.GetConicTransform();

            if (conicSection.ConicSectionType == ConicSectionType.Ellipse)
            {
                return new Ellipse(conicSection, pts[0], pts[1]);
            }

            return conicSection;
        }

        private Transform GetTransform(Plane targetPlane)
        {
            return Transform.PlaneToPlane(Plane.WorldXY, targetPlane);
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
