using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Collections;
using Rhino.Geometry;

namespace the_Dominion.Utility
{
    public class Logo
    {
        public Logo()
        {
            Initialise();
        }

        public Logo(double ratio, double thickness, IEnumerable<double> offsets = null)
            : this(Plane.WorldXY, ratio, thickness, offsets) { }

        public Logo(Plane plane, double ratio, double thickness, IEnumerable<double> offsets = null)
        {
            Ratio = ratio;

            switch (true)
            {
                case var _ when thickness > 1:
                    Thickness = 1;
                    break;
                case var _ when thickness < 0:
                    Thickness = 0;
                    break;
                default:
                    Thickness = thickness;
                    break;
            }

            if (offsets != null)
                Offsets = offsets.ToArray();

            Transformation = Geometry.WorldXYToPlaneTransform(plane);

            Initialise();
        }

        public double Ratio { get; } = 1.25;

        public double Thickness { get; } = 0.2;

        public double[] Offsets { get; } = new double[0];

        public Curve MainShape { get; private set; }

        public CurveList Borders { get; private set; } = new CurveList();

        public Transform Transformation { get; }

        public void Initialise()
        {
            MakeLogo();
            Transform();
        }

        public void MakeLogo()
        {
            double sin30 = Math.Sin(Math.PI / 6);
            double cos30 = Math.Cos(Math.PI / 6);
            double tan30 = Math.Tan(Math.PI / 6);

            var appendDist = (Ratio - 1) / 2;

            var thicknessCapped = Thickness < cos30
                ? Thickness
                : cos30;

            double outerThickness = 1 - (thicknessCapped / cos30);

            double tSpacingR = (cos30 - thicknessCapped) / 2;
            double tSpacingL = (cos30 + thicknessCapped) / 2;

            Point3d p0 = new Point3d(-cos30, sin30, 0);

            Point3d p1 = new Point3d(0, 1, 0);
            Point3d p2 = new Point3d(cos30, sin30, 0);
            Point3d p3 = new Point3d(cos30, -sin30, 0);
            Point3d p4 = new Point3d(0, -1, 0);
            Point3d p5 = new Point3d(0, -outerThickness, 0);
            Point3d p6 = p3 * outerThickness;
            Point3d p7 = p2 * outerThickness;
            Point3d p8 = p1 * outerThickness;
            Point3d p9 = new Point3d(-tSpacingR, p8.Y - tan30 * tSpacingR, 0);
            Point3d p10 = new Point3d(-tSpacingR, -p9.Y, 0);
            Point3d p11 = new Point3d(-tSpacingL, p5.Y + tan30 * tSpacingL, 0);
            Point3d p12 = new Point3d(-tSpacingL, -p11.Y, 0);
            Point3d p13 = new Point3d(p0.X, p0.Y - thicknessCapped / cos30, 0);

            Vector3d appendVector = new Vector3d(0, appendDist, 0);

            p0 += appendVector;
            p1 += appendVector;
            p2 += appendVector;
            p3 -= appendVector;
            p4 -= appendVector;
            p5 -= appendVector;
            p6 -= appendVector;
            p7 += appendVector;
            p8 += appendVector;
            p9 += appendVector;
            p10 -= appendVector;
            p11 -= appendVector;
            p12 += appendVector;
            p13 += appendVector;

            Polyline logoMain;

            if (thicknessCapped < cos30)
                logoMain = new Polyline() { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p0 };
            else
            {
                if (Ratio < 1)
                {
                    Vector3d intersection = new Vector3d(appendDist / tan30, appendDist, 0);
                    p0 -= intersection;

                    logoMain = new Polyline() { p0, p1, p2, p3, p4, p5, p0 };
                }
                else
                {
                    logoMain = new Polyline() { p0, p1, p2, p3, p4, p5, p11, p0 };
                }
            }

            MainShape = logoMain.ToPolylineCurve();
            AddBorder();


        }

        private void AddBorder()
        {
            if (Offsets.Length < 2)
                return;

            var poly = CalculatePolygon();

            for (int i = 0; i < Offsets.Length / 2; i++)
            {
                Curve offset1 = CalculatePolygon(Offsets[i * 2]);
                Curve offset2 = CalculatePolygon(Offsets[i * 2 + 1]);

                Borders.AddRange(new[] { offset1, offset2 });
            }
        }

        public Curve CalculatePolygon(double offset = 0, int nSides = 6)
        {
            Polyline polygon = new Polyline();

            var appendDist = (Ratio - 1) / 2;

            for (int i = 0; i <= nSides; i++)
            {
                double angle = i * Math.PI / 3 + Math.PI / 6;
                double x = Math.Cos(angle);
                double y = Math.Sin(angle);

                if (i % 6 < 3)
                    y += appendDist;
                else
                    y -= appendDist;

                var pt = new Point3d(x, y, 0) * (1 + offset);
                polygon.Add(pt);
            }

            return polygon.ToPolylineCurve();
        }

        private void Transform()
        {
            MainShape.Transform(Transformation);
            Borders.Transform(Transformation);
        }
    }
}
