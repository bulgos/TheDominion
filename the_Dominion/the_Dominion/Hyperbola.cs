using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace the_Dominion
{
    public class Hyperbola
    {
        /// <summary>
        /// Draws a hyperbola on the XZ plane, centred at 0,0,0.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        public static NurbsCurve ComputeHyperbola(double a, double c, double h)
        {
            Point3d p0 = new Point3d(0, 0, 0);
            Point3d p1 = new Point3d(0, 0, 0);
            Point3d p2 = new Point3d(0, 0, 0);
            Point3d q = new Point3d(0, 0, 0);

            p0.X = Math.Sqrt((a * a) * (1 + ((h * h) / (c * c))));
            p0.Z = h;

            p1.X = (-1 * (p0.Z * p0.Z) * (a * a)) / ((c * c) * p0.X) + p0.X;
            p1.Z = 0;

            p2.X = p0.X;
            p2.Z = -h;

            q.X = a;
            q.Z = 0;

            double w1 = (p0.X - q.X) / (q.X - p1.X);

            Point3d[] points = [p0, p1, p2];
            NurbsCurve hyperbola = NurbsCurve.Create(false, 2, points);
            hyperbola.Points.SetPoint(1, new Point4d(p1.X, p1.Y, p1.Z, w1));

            return hyperbola;
        }
    }
}
