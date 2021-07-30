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
        private double _discriminant;

        public ConicSection(Point3d[] points)
        {
             
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
        }

        public Plane BasePlane { get; } = Plane.WorldXY;

        public NurbsCurve Section { get; protected set; }

        public Point3d Focus1 { get; protected set; }

        public Point3d Focus2 { get; protected set; }

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
                if (_discriminant == double.NaN)
                    ComputeDiscriminant();

                return _discriminant;
            }
            set => _discriminant = value;
        }

        public ConicSectionType ConicSectionType => GetConicType();

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

        private ConicSectionType GetConicType()
        {
            if (Discriminant > 0)
            {
                return ConicSectionType.Ellipse;
            }

            if (Discriminant < 0)
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
