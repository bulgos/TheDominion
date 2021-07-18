using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace the_Dominion
{
    public class Parabola
    {
        public static NurbsCurve ConstructParabola(double a)
        {
            Interval interval = new Interval(-1, 1);

            return ConstructParabola(a, interval);
        }

        public static NurbsCurve ConstructParabola(double a, Interval interval)
        {
            double y0 = a * Math.Pow(interval.Min, 2);
            double y2 = a * Math.Pow(interval.Max, 2);

            Point3d p0 = new Point3d(interval.Min, y0, 0);
            Point3d p1 = new Point3d();
            Point3d p2 = new Point3d(interval.Max, y2, 0);

            return NurbsCurve.CreateParabolaFromVertex(p1, p0, p2);
        }
    }
}
