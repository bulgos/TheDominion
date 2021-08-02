using System;
using Rhino.Geometry;

namespace the_Dominion.Conics
{
    public class Hyperbola : ConicSection
    {
        public Hyperbola(ConicSection conicSection)
            : base(conicSection)
        {
            if (F == 0)
                throw new ArgumentException("Hyperbola is degenerate, F cannot be zero");
            if (ConicSectionType != ConicSectionType.Hyperbola)
                throw new ArgumentException("Conic does not represent a Hyperbola");

            ConicSection worldAlignedConic = conicSection.WorldAlignedConic;

            MajorAxis = Math.Pow(Math.Abs(worldAlignedConic.A), -0.5);
            MinorAxis = Math.Pow(Math.Abs(worldAlignedConic.C), -0.5);
            Height = 100;

            ComputeHyperbola();
        }

        public Hyperbola(double a, double b, double height)
            : this(Plane.WorldXY, a, b, height) { }

        public Hyperbola(Plane plane, double a, double b, double height)
            : base(plane)
        {
            A = Math.Pow(b, 2);
            C = -Math.Pow(a, 2);
            F = A * C;

            MajorAxis = a;
            MinorAxis = b;
            Height = height;

            ComputeHyperbola();
            ComputeFoci();
        }

        public Hyperbola(Hyperbola hyperbola)
            : base(hyperbola)
        {
            MajorAxis = hyperbola.MajorAxis;
            MinorAxis = hyperbola.MinorAxis;
            Height = hyperbola.Height;
        }

        public double MajorAxis { get; } = 1;

        public double MinorAxis { get; } = 1;

        public double Height { get; private set; } = 10;

        public Point3d Apex { get; private set; } = Point3d.Unset;

        private void ComputeHyperbola()
        {
            double x0 = Math.Sqrt((MajorAxis * MajorAxis) * (1 + ((Height * Height) / (MinorAxis * MinorAxis))));

            Point3d p0 = new Point3d(x0, Height, 0);
            Point3d p1 = new Point3d(MajorAxis, 0, 0);
            Point3d p2 = new Point3d(x0, -Height, 0);
            
            double w1 = x0 / MajorAxis;
            
            Point4d weightedP1 = new Point4d(p1.X, p1.Y, p1.Z, w1);

            Point3d[] points = { p0, p1, p2 };
            NurbsCurve hyperbola = NurbsCurve.Create(false, 2, points);
            hyperbola.Points.SetPoint(1, weightedP1);

            Section = hyperbola;
            TransformShape();

            Apex = p1;
        }

        public override double ComputeDerivative(Point3d pt)
        {
            return (Math.Pow(MinorAxis, 2) * pt.X) / (Math.Pow(MinorAxis, 2) * pt.Y);
        }

        private double ComputeApexWeight(Point3d p0)
        {
            return MajorAxis * MajorAxis / p0.X;
        }

        /// <summary>
        /// Focus for a Hyperbola is given by c^2 = a^2 + b^2
        /// </summary>
        protected override void ComputeFoci()
        {
            double focusDist = Math.Sqrt(MajorAxis * MajorAxis + MinorAxis * MinorAxis);

            Focus1 = new Point3d(-focusDist, 0, 0);
            Focus2 = new Point3d(focusDist, 0, 0);
        }

        public override ConicSection Duplicate()
        {
            return new Hyperbola(this);
        }
    }
}