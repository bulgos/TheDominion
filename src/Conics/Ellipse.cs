﻿using System;
using Rhino.Geometry;

namespace the_Dominion.Conics
{
    public class Ellipse : ConicSection
    {
        public Ellipse(ConicSection conicSection)
            : base(conicSection)
        {
            if (ConicSectionType != ConicSectionType.Ellipse)
                throw new ArgumentException("Conic does not represent an Ellipse");

            ConicSection worldAlignedConic = conicSection.WorldAlignedConic;

            EllipseA = Math.Pow(Math.Abs(worldAlignedConic.A), -0.5);
            EllipseB = Math.Pow(Math.Abs(worldAlignedConic.C), -0.5);

            var ellipse = new Rhino.Geometry.Ellipse(BasePlane, EllipseA, EllipseB);
            Section = ellipse.ToNurbsCurve();
        }

        public double EllipseA { get; private set; }

        public double EllipseB { get; private set; }


    }
}