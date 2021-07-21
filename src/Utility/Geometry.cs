﻿using Rhino.Collections;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace the_Dominion.Utility
{
    public static class Geometry
    {
        public static double VectorAngleInPlane(this Vector3d v, Plane plane)
        {
            plane.Flip();
            return SignedVectorAngle(plane, v, plane.YAxis);
        }

        public static double SignedVectorAngle(Vector3d v1, Vector3d v2)
        {
            if (v1.Z != 0 || v2.Z != 0)
                return double.NaN;

            return VectorAngle(v2) - VectorAngle(v1);
        }

        public static double VectorAngle(this Vector3d v)
        {
            return Math.Atan2(v.Y, v.X);
        }

        public static double SignedVectorAngle(Plane plane, Vector3d v1, Vector3d v2)
        {
            double angle = Math.Acos(v1.DotProduct(v2));
            Vector3d crossProduct = Vector3d.CrossProduct(v1, v2);

            if (DotProduct(plane.Normal, crossProduct) < 0)
            {
                angle *= -1;
            }

            return angle;
        }

        public static double DotProduct(this Vector3d v1, Vector3d v2)
        {
            v1.Unitize();
            v2.Unitize();

            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Interval ComputeTransformedBoundsInPlane(this IEnumerable<Point3d> points, Plane targetPlane)
        {
            return ComputeTransformedBoundsInPlane(points, Plane.WorldXY, targetPlane);
        }

        public static Interval ComputeTransformedBoundsInPlane(this IEnumerable<Point3d> points, Plane sourcePlane, Plane targetPlane)
        {
            Transform xform = Transform.PlaneToPlane(sourcePlane, targetPlane);

            Point3dList transformablePoints = new Point3dList(points);
            transformablePoints.Transform(xform);

            return ComputeBounds(transformablePoints);
        }

        public static Interval ComputeBounds(this IEnumerable<Point3d> points)
        {
            var xValues = points.Select(pt => pt.X);

            return new Interval(xValues.Min(), xValues.Max());
        }
    }
}
