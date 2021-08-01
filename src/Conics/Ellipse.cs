using Rhino.Geometry;
using System;

namespace the_Dominion.Conics
{
    public class Ellipse : ConicSection
    {
        public Ellipse(ConicSection conicSection)
            : base(conicSection)
        {
            ConicSection worldAlignedConic = new ConicSection(conicSection);
            worldAlignedConic.TransformToStandardConic();

            EllipseA = Math.Pow(Math.Abs(worldAlignedConic.A), -0.5);
            EllipseB = Math.Pow(Math.Abs(worldAlignedConic.C), -0.5);

            Plane basePlane = Plane.WorldXY;
            basePlane.Transform(Transform);

            var ellipse = new Rhino.Geometry.Ellipse(basePlane, EllipseA, EllipseB);
            Section = ellipse.ToNurbsCurve();
        }

        public double EllipseA { get; private set; }

        public double EllipseB { get; private set; }


    }
}
