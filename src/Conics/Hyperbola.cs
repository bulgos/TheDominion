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

            Height = 100;

            Initialise();
        }

        public Hyperbola(double a, double b, double height)
            : this(Plane.WorldXY, a, b, height) { }

        public Hyperbola(Plane plane, double a, double b, double height, bool flipAxis = false)
            : base(plane, b * b, 0, -a * a, 0, 0, -a * b)
        {
            if (flipAxis)
            {
                AxisA *= -1;
                AxisB *= -1;
            }

            Height = height;

            Initialise();
        }

        public Hyperbola(Hyperbola hyperbola)
            : base(hyperbola)
        {
            AxisA = hyperbola.AxisA;
            AxisB = hyperbola.AxisB;
            Height = hyperbola.Height;
        }

        public double Height { get; private set; } = 10;

        public Point3d Apex { get; private set; } = Point3d.Unset;

        private void Initialise()
        {
            ComputeHyperbola();
            ComputeFoci();
            Transform(TransformMatrix);
        }

        private void ComputeHyperbola()
        {
            NurbsCurve hyperbola;
            Point3d[] pts;
            double weight;
            Plane mirrorPlane;

            if (AxisA > AxisB)
            {
                double x0 = Math.Sqrt((AxisA * AxisA) * (1 + ((Height * Height) / (AxisB * AxisB))));

                Point3d p0 = new Point3d(x0, -Height, 0);
                Point3d p1 = new Point3d(AxisA, 0, 0);
                Point3d p2 = new Point3d(x0, Height, 0);

                weight = x0 / AxisA;

                pts = new[] { p0, p1, p2 };

                mirrorPlane = Plane.WorldYZ;
            }
            else
            {
                double y0 = Math.Sqrt((AxisB * AxisB) * (1 + ((Height * Height) / (AxisA * AxisA))));

                Point3d p0 = new Point3d(-Height, y0, 0);
                Point3d p1 = new Point3d(0, AxisB, 0);
                Point3d p2 = new Point3d(Height, y0, 0);

                weight = y0 / AxisB;

                pts = new[] { p0, p1, p2 };

                mirrorPlane = Plane.WorldZX;
            }

            Point4d weightedP1 = new Point4d(pts[1].X, pts[1].Y, pts[1].Z, weight);

            Apex = pts[1];

            Transform mirrorXform = Rhino.Geometry.Transform.Mirror(mirrorPlane);

            hyperbola = NurbsCurve.Create(false, 2, pts);

            if (hyperbola == null)
                return;

            hyperbola.Points.SetPoint(1, weightedP1);
            NurbsCurve mirroredHyperbola = hyperbola.Duplicate() as NurbsCurve;
            mirroredHyperbola.Transform(mirrorXform);

            Section.Add(hyperbola);
            Section.Add(mirroredHyperbola);
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
                Focus1 = new Point3d(0, -focusDist, 0);
                Focus2 = new Point3d(0, focusDist, 0);
            }
        }

        public override ConicSection Duplicate()
        {
            return new Hyperbola(this);
        }
    }
}