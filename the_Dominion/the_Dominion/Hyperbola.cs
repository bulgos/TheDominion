using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace the_Dominion
{
    /// <summary>
    /// see https://www.danieldavis.com/how-to-draw-a-hyperboloid/
    /// </summary>
    public class Hyperbola
    {
        /// <summary>
        /// Draws a hyperbola on the XZ plane, centred at 0,0,0
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        public static NurbsCurve ComputeHyperbola(double a, double b, double h)
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

            double x0 = Math.Sqrt((a * a) * (1 + ((h * h) / (b * b))));
            Point3d p0 = new Point3d(x0, 0, h);

            Point3d p2 = new Point3d(x0, 0, -h);

            // p1 is harder to calculate.
            // It lies on the x-axis, at the intersection of the tangents from p0 & 02
            // The tangent for a point(x', y') on a hyperbola is:
            // y-y' = (b^2 * x') / (a^2 * y') * (x - x')
            // http://openstudy.com/updates/4f7ef8c8e4b0bfe8930b75a2
            // Since we know y = 0, we can solve for x
            // x = (-y'^2 * a^2) / (b^2 * x') + x'
            // where x' and y' are the point on the hyperbola 

            double x1 = (-1 * (h * h) * (a * a)) / ((b * b) * p0.X) + p0.X;
            Point3d p1 = new Point3d(x1, 0, 0);

            // We also need the apex point for the hyperbola
            // This is the point where the hyperbola crosses the x-axis

            Point3d q = new Point3d(a, 0, 0);

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
        /// Draws a hyperbola on the XZ plane, centred at 0,0,0 with default a = 1, b = 1, h = 10
        /// </summary>
        /// <returns></returns>
        public static NurbsCurve ComputeHyperbola()
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

            double a = 1;
            double b = 1;
            double height = 10;
            double x = Math.Sqrt(a * a * (1 + ((height * height) / (b * b))));
            Point3d p0 = new Point3d(x, 0, height);
            Point3d p2 = new Point3d(x, 0, -1 * height);

            // p1 is harder to calculate.
            // It lies on the x-axis, at the intersection of the tangents from p0 & 02
            // The tangent for a point(x', y') on a hyperbola is:
            // y-y' = (b^2 * x') / (a^2 * y') * (x - x')
            // http://openstudy.com/updates/4f7ef8c8e4b0bfe8930b75a2
            // Since we know y = 0, we can solve for x
            // x = (-y'^2 * a^2) / (b^2 * x') + x'
            // where x' and y' are the point on the hyperbola 

            x = (-1 * p0.Z * p0.Z * a * a) / (b * b * p0.X) + p0.X;
            Point3d p1 = new Point3d(x, 0, 0);

            // We also need the apex point for the hyperbola
            // This is the point where the hyperbola crosses the x-axis

            Point3d apex = new Point3d(a, 0, 0);

            // To draw the hyperbola, we need to know what weight is given to p1
            // The formula is:

            //d = weight / (1 + weight) * e;
            // http://www.cs.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/NURBS/RB-conics.html

            //where d = the distance between the apex and point_M
            // and e = the distance between p1 and point_M
            // and point_M is midway between p0 and p2
            // thus, the weight = d / (e - d) 

            double point_M = p0.X;
            double d = point_M - apex.X;
            double e = point_M - p1.X;

            double weight = d / (e - d);

            // create hyperbola
            Point3d[] points = { p0, p1, p2 };
            NurbsCurve hyperbola = NurbsCurve.Create(false, 2, points);
            hyperbola.Points.SetPoint(1, new Point4d(p1.X, p1.Y, p1.Z, weight));

            return hyperbola;
        }
    }
}
