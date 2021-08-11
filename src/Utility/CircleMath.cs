using Rhino.Geometry;
using System;

namespace the_Dominion.Utility
{
    public class CircleMath
    {
        // reference for tangent circles 
        // https://math.stackexchange.com/questions/719758/inner-tangent-between-two-circles-formula

        public static Line[] GetOuterTangets(Circle circle1, Circle circle2)
        {
            double r3 = circle1.Radius - circle2.Radius;

            double hypotenuse = circle1.Center.DistanceTo(circle2.Center);
            double shortSide = r3;

            double angle1 = Math.Atan2(circle2.Center.Y - circle1.Center.Y, circle2.Center.X - circle1.Center.X) + Math.Acos(shortSide / hypotenuse);
            double angle2 = Math.Atan2(circle2.Center.Y - circle1.Center.Y, circle2.Center.X - circle1.Center.X) - Math.Acos(shortSide / hypotenuse);

            Circle circleWorld1 = new Circle(circle1.Center, circle1.Radius);
            Circle circleWorld2 = new Circle(circle2.Center, circle2.Radius);

            Point3d t1 = circleWorld1.PointAt(angle1);
            Point3d t2 = circleWorld2.PointAt(angle1);

            Point3d t3 = circleWorld1.PointAt(angle2);
            Point3d t4 = circleWorld2.PointAt(angle2);

            Line line1 = new Line(t1, t2);
            Line line2 = new Line(t3, t4);

            return new Line[] { line1, line2 };
        }

        public static Line[] GetInnerTangents(Circle circle1, Circle circle2)
        {
            double hypotenuse = circle1.Center.DistanceTo(circle2.Center);
            double shortSide = circle1.Radius + circle2.Radius;

            double angle1 = Math.Atan2(circle2.Center.Y - circle1.Center.Y, circle2.Center.X - circle1.Center.X) + Math.Asin(shortSide / hypotenuse) - Math.PI / 2;
            double angle2 = Math.Atan2(circle2.Center.Y - circle1.Center.Y, circle2.Center.X - circle1.Center.X) - Math.Asin(shortSide / hypotenuse) + Math.PI / 2;

            Circle circleWorld1 = new Circle(circle1.Center, circle1.Radius);
            Circle circleWorld2 = new Circle(circle2.Center, circle2.Radius);

            Point3d t1 = circleWorld1.PointAt(angle1);
            Point3d t2 = circleWorld2.PointAt(angle1 + Math.PI);

            Point3d t3 = circleWorld1.PointAt(angle2);
            Point3d t4 = circleWorld2.PointAt(angle2 + Math.PI);

            Line line1 = new Line(t1, t2);
            Line line2 = new Line(t3, t4);

            return new Line[] { line1, line2 };
        }
    }
}
