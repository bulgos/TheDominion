using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Rhino.Collections;
using Rhino.Geometry;
using System;

namespace the_Dominion.Conics
{
    public class Ellipse : ConicSection
    {
        public Ellipse(ConicSection conicSection, Point3d pt1, Point3d pt2)
            : base(conicSection)
        {
            Point3dList pointList = new Point3dList(new[] { pt1, pt2 });
            

            pointList.Transform(InverseTransform);

            var ellipseMatrixValues = new double[2][];
            Vector<double> ellipseVector = Vector.Build.Dense(2, 1);

            for (int i = 0; i < 2; i++)
            {
                ellipseMatrixValues[i] = new[] { Math.Pow(pointList[i].X, 2), Math.Pow(pointList[i].Y, 2) };
            }

            Matrix<double> matrix = DenseMatrix.OfRowArrays(ellipseMatrixValues);
            //var solution = matrix.Solve(ellipseVector);
            var solution = matrix.Solve(ellipseVector);

            EllipseA = Math.Sqrt(1 / solution[0]);
            EllipseB = Math.Sqrt(1 / solution[1]);

            Plane basePlane = Plane.WorldXY;
            basePlane.Transform(Transform);

            var ellipse = new Rhino.Geometry.Ellipse(basePlane, EllipseA, EllipseB);
            Section = ellipse.ToNurbsCurve();
        }

        public double EllipseA { get; private set; }

        public double EllipseB { get; private set; }


    }
}
