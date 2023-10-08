using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Rhino.Collections;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Dominion.Core.Utility;

namespace Dominion.Core.Conics
{
    public class Parabola : ConicSection
    {
        private double _parabolaDiscriminant = double.NaN;
        private Point3d[] _roots = null;

        public Parabola(ConicSection conicSection)
            : base(conicSection)
        {
            if (ConicSectionType != ConicSectionType.Parabola)
                throw new ArgumentException("Conic does not represent a Parabola");

            Initialise();
        }

        public Parabola(double a, Interval domain)
            : this(a, 0, 0, Plane.Unset, domain) { }

        public Parabola(double a, double b, double c, Plane plane, Interval domain)
            : base(plane, a, 0, 0, b, -1, c)
        {
            if (domain != Interval.Unset)
                Domain = domain;

            Initialise();
        }

        public Parabola(Parabola parabola)
            : base(parabola)
        {
            Domain = parabola.Domain;
            VertexPlane = parabola.VertexPlane;
        }

        public Interval Domain { get; } = new Interval(-10, 10);

        public Plane VertexPlane { get; private set; }

        public Point3d[] Roots
        {
            get
            {
                if (_roots == null)
                {
                    ComputeQuadraticRoots();
                }

                return _roots;
            }
            private set => _roots = value;
        }

        public double ParabolaDiscriminant
        {
            get
            {
                if (double.IsNaN(_parabolaDiscriminant))
                    ComputeParabolaDiscriminant();

                return _parabolaDiscriminant;
            }
            set => _parabolaDiscriminant = value;
        }

        public override bool UsesFlippedAxes => AxisB != 0;

        public ParabolaShape ParabolaShape => GetParabolaShape();

        public void Initialise()
        {
            ConstructParabola();
            ComputeVertexPlane();
            ComputeFoci();
            Transform(TransformMatrix, true, true, false);
        }

        public void ConstructParabola()
        {
            Point3d p0;
            Point3d p2;

            if (!UsesFlippedAxes)
            {
                p0 = WorldAlignedConic.ComputePointAtX(Domain.Min)[0];
                p2 = WorldAlignedConic.ComputePointAtX(Domain.Max)[0];
            }
            else
            {
                p0 = WorldAlignedConic.ComputePointAtY(Domain.Min)[0];
                p2 = WorldAlignedConic.ComputePointAtY(Domain.Max)[0];
            }

            Line tangent1 = ComputeTangent(p0);
            Line tangent2 = ComputeTangent(p2);

            Point3d p1 = Geometry.ComputeLineIntersection(tangent1, tangent2);

            Point3d[] points = { tangent1.From, p1, tangent2.From };

            NurbsCurve crv = Curve.CreateControlPointCurve(points, 2) as NurbsCurve;

            if (crv == null)
                return;

            Section.Add(crv);
        }

        public override Line ComputeTangent(Point3d pt)
        {
            double derivative = ComputeDerivative(pt);

            Vector3d direction = !UsesFlippedAxes
                ? new Vector3d(1, derivative, 0)
                : new Vector3d(derivative, 1, 0);
            direction.Unitize();

            return new Line(pt, direction);
        }

        protected override void ComputeFoci()
        {
            double vertexParameter;

            if (UsesFlippedAxes)
                vertexParameter = Math.Abs(AxisB);
            else
                vertexParameter = Math.Abs(AxisA);

            Focus1 = VertexPlane.Origin + VertexPlane.YAxis * vertexParameter / 4;
        }

        public void ComputeParabolaPoint(ref Point3d pt)
        {
            if (UsesFlippedAxes)
                pt.X = WorldAlignedConic.C * pt.Y * pt.Y + WorldAlignedConic.E * pt.Y + WorldAlignedConic.F;
            else
                pt.Y = WorldAlignedConic.A * pt.X * pt.X + WorldAlignedConic.D * pt.X + WorldAlignedConic.F;
        }

        public Point3d ComputeParabolaVertex()
        {
            Point3d pt = new Point3d();

            if (UsesFlippedAxes)
                pt.Y = -WorldAlignedConic.E / (2 * WorldAlignedConic.C);
            else
                pt.X = -WorldAlignedConic.D / (2 * WorldAlignedConic.A);

            ComputeParabolaPoint(ref pt);

            return pt;
        }

        private void ComputeVertexPlane()
        {
            var vertex = ComputeParabolaVertex();

            double angle = Math.PI / 2;

            switch (ParabolaShape)
            {
                case ParabolaShape.NegativeX:
                    break;
                case ParabolaShape.NegativeY:
                    angle *= 2;
                    break;
                case ParabolaShape.PositiveX:
                    angle *= 3;
                    break;
                case ParabolaShape.PositiveY:
                    angle = 0;
                    break;
            }

            var vertexPlane = Plane.WorldXY;
            vertexPlane.Rotate(angle, vertexPlane.ZAxis);
            vertexPlane.Origin = vertex;

            VertexPlane = vertexPlane;
        }

        protected override void TransformProperties(Transform xform)
        {
            base.TransformProperties(xform);

            Plane vertexPlane = VertexPlane;
            vertexPlane.Transform(TransformMatrix);
            VertexPlane = vertexPlane;
        }

        private void ComputeQuadraticRoots()
        {
            Roots = ComputePointAtY(0);
        }

        private void ComputeParabolaDiscriminant()
        {
            ParabolaDiscriminant = Geometry.ComputeDiscriminant(A, D, F);
        }

        private ParabolaShape GetParabolaShape()
        {
            if (AxisB == 0)
            {
                if (WorldAlignedConic.A * WorldAlignedConic.E < 0)
                    return ParabolaShape.PositiveY;
                else
                    return ParabolaShape.NegativeY;
            }
            else
            {
                if (WorldAlignedConic.C * WorldAlignedConic.D < 0)
                    return ParabolaShape.PositiveX;
                else
                    return ParabolaShape.NegativeX;
            }
        }

        public override object Clone() 
            => new Parabola(this);
    }

    public enum ParabolaShape
    {
        NegativeX,
        NegativeY,
        PositiveX,
        PositiveY
    }
}