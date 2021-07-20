using Rhino.Geometry;

namespace the_Dominion.Conics
{
    public abstract class ConicSection
    {
        private Transform _inverseTransform = Transform.Unset;

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

        public BoundingBox BoundingBox => 
            IsValid 
            ? Section.GetBoundingBox(Transform) 
            : BoundingBox.Empty;

        public virtual bool IsValid => Section != null;

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
            if (!IsValid || xform == Transform.Identity)
                return;

            Section.Transform(xform);

            Point3d focus = Focus;
            focus.Transform(xform);
            Focus = focus;
        }

        public abstract ConicSection Duplicate();

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
    }
}
