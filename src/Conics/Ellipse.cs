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

            MajorAxis = Math.Pow(Math.Abs(worldAlignedConic.A), -0.5);
            MinorAxis = Math.Pow(Math.Abs(worldAlignedConic.C), -0.5);

            var ellipse = new Rhino.Geometry.Ellipse(BasePlane, MajorAxis, MinorAxis);
            Section = ellipse.ToNurbsCurve();
        }

        public double MajorAxis { get; private set; }

        public double MinorAxis { get; private set; }

        public override double ComputeDerivative(Point3d pt)
        {
            return Math.Pow(MajorAxis, 2) * pt.Y / (Math.Pow(MinorAxis, 2) * pt.X);
        }

        public override ConicSection Duplicate()
        {
            return new Ellipse(this);
        }

        protected override void ComputeFoci()
        {
            if (MajorAxis == MinorAxis)
            {
                Focus1 = Point3d.Origin;
            }

            var focusDist = Math.Sqrt(MajorAxis * MajorAxis - MinorAxis * MinorAxis);

            Focus1 = new Point3d(-focusDist, 0, 0);
            Focus2 = new Point3d(focusDist, 0, 0);
        }
    }
}
