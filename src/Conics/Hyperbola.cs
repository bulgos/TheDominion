using System;
using Rhino.Geometry;

namespace the_Dominion.Conics
{
    /// <summary>
    /// see https://www.danieldavis.com/how-to-draw-a-hyperboloid/
    /// </summary>
    public class Hyperbola : ConicSection
    {
        public Hyperbola()
            : this(1, 1, 10) { }

        public Hyperbola(double a, double b, double h)
            : this(Plane.WorldXY, a, b, h) { }

        public Hyperbola(Plane plane, double a, double b, double h)
            : base(plane)
        {
            A = a;
            B = b;
            H = h;

            Section = ComputeHyperbola();
            ComputeFocus();

            TransformShape();
        }

        public Hyperbola(Hyperbola hyperbola)
            : base(hyperbola)
        {
            A = hyperbola.A;
            B = hyperbola.B;
            H = hyperbola.H;
        }

        public double A { get; }

        public double B { get; }

        public double H { get; }

        /// <summary>
        /// Draws a hyperbola on the XY plane, centred at 0,0,0
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        private NurbsCurve ComputeHyperbola()
        {
            // A hyperbola can be drawn as a quadratic rational bezier curve
            // The curve has three control points: p0, p1, p2
            // p0 & 02 are the end points of the curve
            // p1 is near the apex of the curve
            //
            // Since the hyperboloid is being generated about the z-axis, we can assume
            // p0 & p2 are mirror images of one another about the xy plane.
            // p0 & p2 can be calculated directly from the hyperbola formula:
            // x^2 / a^2 - y^2 / b^2 = 1
            // In this case, because we are drawing vertically, y = z = height of the hyperbola
            // so a, b, and z are known. Thus:
            // x^2 = a^2 * (1 + (z^2 / b^2)) 

            double x0 = Math.Sqrt((A * A) * (1 + ((H * H) / (B * B))));
            Point3d p0 = new Point3d(x0, H, 0);

            Point3d p2 = new Point3d(x0, -H, 0);

            // p1 is harder to calculate.
            // It lies on the x-axis, at the intersection of the tangents from p0 & 02
            // The tangent for a point(x', y') on a hyperbola is:
            // y-y' = (b^2 * x') / (a^2 * y') * (x - x')
            // http://openstudy.com/updates/4f7ef8c8e4b0bfe8930b75a2
            // Since we know y = 0, we can solve for x
            // x = (-y'^2 * a^2) / (b^2 * x') + x'
            // where x' and y' are the point on the hyperbola 

            double x1 = (-1 * (H * H) * (A * A)) / ((B * B) * p0.X) + p0.X;
            Point3d p1 = new Point3d(x1, 0, 0);

            // We also need the apex point for the hyperbola
            // This is the point where the hyperbola crosses the x-axis

            Point3d q = new Point3d(A, 0, 0);

            // To draw the hyperbola, we need to know what weight is given to p1

            double w1 = (p0.X - q.X) / (q.X - p1.X);

            //d = weight / (1 + weight) * e;
            // http://www.cs.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/NURBS/RB-conics.html

            //where d = the distance between the apex and point_M
            // and e = the distance between p1 and point_M
            // and point_M is midway between p0 and p2
            // thus, the weight = d / (e - d) 

            Point3d[] points = { p0, p1, p2 };
            NurbsCurve hyperbola = NurbsCurve.Create(false, 2, points);
            hyperbola.Points.SetPoint(1, new Point4d(p1.X, p1.Y, p1.Z, w1));

            return hyperbola;
        }

        /// <summary>
        /// Focus for a Hyperbola is given by c^2 = a^2 + b^2
        /// </summary>
        protected override void ComputeFocus()
        {
            double focusDist = Math.Sqrt(A * A + B * B);

            Focus1 = new Point3d(focusDist, 0, 0);
        }

        public override ConicSection Duplicate()
        {
            return new Hyperbola(this);
        }
    }
}
