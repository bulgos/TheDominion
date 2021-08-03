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

            ConicSection worldAlignedConic = conicSection.WorldAlignedConic;

            AxisA = Math.Pow(Math.Abs(worldAlignedConic.A), -0.5);
            AxisB = Math.Pow(Math.Abs(worldAlignedConic.C), -0.5);

            var ellipse = new Rhino.Geometry.Ellipse(BasePlane, AxisA, AxisB);
            Section = ellipse.ToNurbsCurve();
        }

        public double AxisA { get; private set; }

        public double AxisB { get; private set; }

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

            var focusDist = Math.Sqrt(AxisA * AxisA - AxisB * AxisB);

            Focus1 = new Point3d(-focusDist, 0, 0);
            Focus2 = new Point3d(focusDist, 0, 0);
        }
    }
}
