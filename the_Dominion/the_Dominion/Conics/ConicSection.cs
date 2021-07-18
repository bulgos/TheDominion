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
            {
                Transform = Transform.Identity;
            }
            else
            {
                BasePlane = plane;
                Transform = GetTransform(plane);
            }
        }

        public Plane BasePlane { get; protected set; } = Plane.WorldXY;

        public NurbsCurve Section { get; protected set; }

        public Point3d Focus { get; protected set; }

        public ConicSectionType ConicSectionType => Section.GetConicSectionType();

        protected Transform Transform { get; }

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
        protected abstract void ComputeFocus();

        private Transform GetTransform(Plane targetPlane)
        {
            return Transform.PlaneToPlane(Plane.WorldXY, targetPlane);
        }

        public void TransformShape()
        {
            if (Transform == Transform.Identity)
                return;

            Section.Transform(Transform);

            Point3d focus = Focus;
            focus.Transform(Transform);
            Focus = focus;

            Plane basePlane = BasePlane;
            basePlane.Transform(Transform);
            BasePlane = basePlane;
        }
    }
}
