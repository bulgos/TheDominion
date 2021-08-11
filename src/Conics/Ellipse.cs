using System;
using Rhino.Geometry;

namespace the_Dominion.Conics
{
    public class Ellipse : ConicSection
    {
        public Ellipse(ConicSection conicSection)
            : base(conicSection)
        {
            if (F == 0)
                throw new ArgumentException("Ellipse is degenerate, F cannot be zero");
            if (ConicSectionType != ConicSectionType.Ellipse && ConicSectionType != ConicSectionType.Circle)
                throw new ArgumentException("Conic does not represent an Ellipse");

            Initialise();
        }

        public Ellipse(Ellipse ellipse)
            : base(ellipse) { }

        public void Initialise()
        {
            Rhino.Geometry.Ellipse ellipse = new Rhino.Geometry.Ellipse(BasePlane, AxisA, AxisB);
            
            NurbsCurve crv = ellipse.ToNurbsCurve();
            
            if (crv == null)
                return;

            Section.Add(crv);

            ComputeFoci();
        }

        public override double ComputeDerivative(Point3d pt)
        {
            return Math.Pow(AxisA, 2) * pt.Y / (Math.Pow(AxisB, 2) * pt.X);
        }

        public override ConicSection Duplicate()
        {
            return new Ellipse(this);
        }

        protected override void ComputeFoci()
        {
            if (AxisA == AxisB)
            {
                Focus1 = Point3d.Origin;
            }

            double focusDist = Math.Sqrt(Math.Abs(AxisA * AxisA - AxisB * AxisB));

            if (Math.Abs(AxisA) > Math.Abs(AxisB))
            {
                Focus1 = BasePlane.Origin - BasePlane.XAxis * focusDist;
                Focus2 = BasePlane.Origin + BasePlane.XAxis * focusDist;
            }
            else
            {
                Focus1 = BasePlane.Origin - BasePlane.YAxis * focusDist;
                Focus2 = BasePlane.Origin + BasePlane.YAxis * focusDist;
            }
        }
    }
}
