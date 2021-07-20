using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace the_Dominion.Conics
{
    public abstract class ConicSection
    {
        private Transform _inverseTransform = Transform.Unset;

        protected ConicSection()
            : this(Plane.WorldXY) { }

        protected ConicSection(Plane plane)
        {
            if (plane == Plane.WorldXY)
                return;

            BasePlane = plane;
            Transform = GetTransform(plane);
        }

        public ConicSection(ConicSection conicSection)
            : this(conicSection.BasePlane) { }

        public Plane BasePlane { get; } = Plane.WorldXY;

        public NurbsCurve Section { get; protected set; }

        public Point3d Focus { get; protected set; }

        public ConicSectionType ConicSectionType => Section.GetConicSectionType();

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

        public BoundingBox BoundingBox => Section.GetBoundingBox(Transform);

        protected abstract void ComputeFocus();

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
            if (xform == Transform.Identity)
                return;

            Section.Transform(xform);

            Point3d focus = Focus;
            focus.Transform(xform);
            Focus = focus;
        }

        public abstract ConicSection Duplicate();

        public BoundingBox GetBoundingBox(Transform xform)
        {
            return Section.GetBoundingBox(xform);
        }

        public bool Morph(SpaceMorph xmorph)
        {
            return xmorph.Morph(Section);
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
