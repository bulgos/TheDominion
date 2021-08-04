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

            AxisA = Math.Pow(Math.Abs(worldAlignedConic.A), -0.5);
            AxisB = -Math.Pow(Math.Abs(worldAlignedConic.C), -0.5);

            Height = 100;

            ComputeHyperbola();
        }

        public Hyperbola(double a, double b, double height)
            : this(Plane.WorldXY, a, b, height) { }

        public Hyperbola(Plane plane, double a, double b, double height, bool flipAxis = false)
            : base(plane)
            {
            A = Math.Pow(b, 2);
            C = -Math.Pow(a, 2);
            F = A * C;

            AxisA = Math.Abs(a);
            AxisB = -Math.Abs(b);
            
            if (flipAxis)
            {
                AxisA *= -1;
                AxisB *= -1;
            }

            Height = height;

            ComputeHyperbola();
            ComputeFoci();
        }

        public Hyperbola(Hyperbola hyperbola)
            : base(hyperbola)
        {
            AxisA = hyperbola.AxisA;
            AxisB = hyperbola.AxisB;
            Height = hyperbola.Height;
        }

        public double AxisA { get; } = 1;

        public double AxisB { get; } = 1;

        public double Height { get; private set; } = 10;

        public Point3d Apex { get; private set; } = Point3d.Unset;

        private void ComputeHyperbola()
        {
            NurbsCurve hyperbola;
            Point3d[] pts;
            double weight = 1;

            if (AxisA > AxisB)
            {
                double x0 = Math.Sqrt((AxisA * AxisA) * (1 + ((Height * Height) / (AxisB * AxisB))));

                Point3d p0 = new Point3d(x0, Height, 0);
                Point3d p1 = new Point3d(AxisA, 0, 0);
                Point3d p2 = new Point3d(x0, -Height, 0);

                weight = x0 / AxisA;

                pts = new[] { p0, p1, p2 };
            }
            else
            {
                double y0 = Math.Sqrt((AxisB * AxisB) * (1 + ((Height * Height) / (AxisA * AxisA))));

                Point3d p0 = new Point3d(Height, y0, 0);
                Point3d p1 = new Point3d(0, AxisB, 0);
                Point3d p2 = new Point3d(-Height, y0, 0);

                weight = y0 / AxisB;

                pts = new[] { p0, p1, p2 };
            }

            Point4d weightedP1 = new Point4d(pts[1].X, pts[1].Y, pts[1].Z, weight);

            hyperbola = NurbsCurve.Create(false, 2, pts);
            hyperbola.Points.SetPoint(1, weightedP1);

            Apex = pts[1];

            hyperbola.Transform(TransformMatrix);
            Section = hyperbola;
        }

        public override double ComputeDerivative(Point3d pt)
        {
            return (Math.Pow(AxisB, 2) * pt.X) / (Math.Pow(AxisB, 2) * pt.Y);
        }

        private double ComputeApexWeight(Point3d p0)
        {
            return AxisA * AxisA / p0.X;
        }

        /// <summary>
        /// Focus for a Hyperbola is given by c^2 = a^2 + b^2
        /// </summary>
        protected override void ComputeFoci()
        {
            double focusDist = Math.Sqrt(AxisA * AxisA + AxisB * AxisB);
            
            if (AxisA > AxisB)
            {
                Focus1 = new Point3d(-focusDist, 0, 0);
                Focus2 = new Point3d(focusDist, 0, 0);
            }
            else
            {
                Focus1 = new Point3d(0, -focusDist,0);
                Focus2 = new Point3d(0, focusDist, 0);
            }
        }

        public override ConicSection Duplicate()
        {
            return new Hyperbola(this);
        }
    }
}