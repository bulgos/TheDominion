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

        public Logo(double width, double height, double thickness, IEnumerable<double> offsets = null)
        {
            Width = width;
            Height = height;

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

            Initialise();
        }

        public double Width { get; } = 1;

        public double Height { get; } = 1.25;

        public double Thickness { get; } = 0.2;

        public double[] Offsets { get; } = new double[0];

        public CurveList Shape { get; private set; } = new CurveList();

        public void Initialise()
        {
            MakeLogo();
        }

        public void MakeLogo()
        {
            double sin30 = Math.Sin(Math.PI / 6);
            double cos30 = Math.Cos(Math.PI / 6);
            double tan30 = Math.Tan(Math.PI / 6);

            double outerThickness = 1 - (Thickness / cos30);

            double tSpacingR = (cos30 - Thickness) / 2;
            double tSpacingL = (cos30 + Thickness) / 2;

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
            Point3d p13 = new Point3d(p0.X, p0.Y - Thickness / cos30, 0);

            Polyline logoMain;

            if (Thickness < cos30)
                logoMain = new Polyline() { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p0 };
            else
                logoMain = new Polyline() { p0, p1, p2, p3, p4, Point3d.Origin, p0 };

            Shape.Add(logoMain);
            AddBorder();

            Transform xform = Transform.Scale(Plane.WorldXY, Width, Height, 1);

            Shape.Transform(xform);
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

                Shape.AddRange(new[] { offset1, offset2 });
            }
        }

        public Curve CalculatePolygon(double offset = 0, int nSides = 6)
        {
            Polyline polygon = new Polyline();

            for (int i = 0; i <= nSides; i++)
            {
                double angle = i * Math.PI / 3 + Math.PI / 6;
                double x = Math.Cos(angle);
                double y = Math.Sin(angle);

                var pt = new Point3d(x, y, 0) * (1 + offset);
                polygon.Add(pt);
            }

            return polygon.ToPolylineCurve();
        }
    }
}
