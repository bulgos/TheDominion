using Rhino.Geometry;
using Rhino.Geometry.Intersect;
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

        public static NurbsCurve ConstructParabolaFromFocus(double a, Interval interval)
        {
            Point3d focus = new Point3d(0, a / 4, 0);

            double y0 = a * Math.Pow(interval.Min, 2);
            double y1 = a * Math.Pow(interval.Max, 2);

            Point3d p0 = new Point3d(interval.Min, y0, 0);
            Point3d p1 = new Point3d(interval.Max, y1, 0);

            return NurbsCurve.CreateParabolaFromFocus(focus, p0, p1);
        }

        public static Curve ConstructCustomParabola(double a, Interval interval)
        {
            double derivate = 2 * a;

            double y1 = a * Math.Pow(interval.Min, 2);
            double y2 = a * Math.Pow(interval.Max, 2);

            Point3d p1 = new Point3d(interval.Min, y1, 0);
            Point3d p2 = new Point3d(interval.Max, y2, 0);

            Vector3d t1 = new Vector3d(1, derivate * interval.Min, 0);
            Vector3d t2 = new Vector3d(1, derivate * interval.Max, 0);

            Line tLine1 = new Line(p1, p1 + t1);
            Line tLine2 = new Line(p2, p2 + t2);

            Intersection.LineLine(tLine1, tLine2, out double param1, out double param2);

            Point3d p0 = tLine1.PointAt(param1);

            Point3d[] points = { p1, p0, p2 };

            return Curve.CreateControlPointCurve(points, 2);
        }
    }
}
