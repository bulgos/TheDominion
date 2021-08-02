using System;
using Rhino.Geometry;

namespace the_Dominion.Conics
{
    /// <summary>
    /// see https://www.danieldavis.com/how-to-draw-a-hyperboloid/
    /// </summary>
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

            HyperbolaA = Math.Pow(Math.Abs(worldAlignedConic.A), -0.5);
            HyperbolaB = Math.Pow(Math.Abs(worldAlignedConic.C), -0.5);
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

            HyperbolaA = a;
            HyperbolaB = b;
            Height = height;

            ComputeHyperbola();
            ComputeFocus();
        }

        public Hyperbola(Hyperbola hyperbola)
            : base(hyperbola)
        {
            HyperbolaA = hyperbola.HyperbolaA;
            HyperbolaB = hyperbola.HyperbolaB;
            Height = hyperbola.Height;
        }

        public double HyperbolaA { get; } = 1;

        public double HyperbolaB { get; } = 1;

        public double Height { get; private set; } = 10;

        public Point3d Apex { get; private set; } = Point3d.Unset;

        private Line ComputeTangent(Point3d pt)
        {
            double derivative = (HyperbolaB * HyperbolaB * pt.X) / (HyperbolaA * HyperbolaA * pt.Y);

            var direction = new Vector3d(1, derivative, 0);

            return new Line(pt, direction);
        }

        private void ComputeHyperbola()
        {
            double x0 = Math.Sqrt((HyperbolaA * HyperbolaA) * (1 + ((Height * Height) / (HyperbolaB * HyperbolaB))));

            Point3d p0 = new Point3d(x0, Height, 0);
            Point3d p1 = new Point3d(HyperbolaA, 0, 0);
            Point3d p2 = new Point3d(x0, -Height, 0);

            
            double w1 = x0 / HyperbolaA;
            
            Point4d weightedP1 = new Point4d(p1.X, p1.Y, p1.Z, w1);

            Point3d[] points = { p0, p1, p2 };
            NurbsCurve hyperbola = NurbsCurve.Create(false, 2, points);
            hyperbola.Points.SetPoint(1, weightedP1);

            TransformShape();

            Section = hyperbola;

            Apex = p1;
        }

        private double ComputeApexWeight(Point3d p0)
        {
            return HyperbolaA * HyperbolaA / p0.X;
        }

        /// <summary>
        /// Focus for a Hyperbola is given by c^2 = a^2 + b^2
        /// </summary>
        protected override void ComputeFocus()
        {
            double focusDist = Math.Sqrt(HyperbolaA * HyperbolaA + HyperbolaB * HyperbolaB);

            Focus1 = new Point3d(focusDist, 0, 0);
        }

        public override ConicSection Duplicate()
        {
            return new Hyperbola(this);
        }
    }
}
