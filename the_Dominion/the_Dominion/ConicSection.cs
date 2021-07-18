using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace the_Dominion
{
    public abstract class ConicSection 
    {
        public NurbsCurve Section { get; protected set; }

        public Point3d Focus { get; protected set; }

        public ConicSectionType ConicSectionType => Section.GetConicSectionType();

        protected abstract void ComputeFocus();

        public void TransformShape(Plane source, Plane target)
        {
            Transform xform = Transform.PlaneToPlane(source, target);
            TransformShape(xform);
        }

        public void TransformShape(Transform xform)
        {
            Section.Transform(xform);
            
            Point3d focus = Focus;
            focus.Transform(xform);

            Focus = focus;
        }
    }
}
