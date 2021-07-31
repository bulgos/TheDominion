using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Solvers;
using Rhino.Collections;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace the_Dominion.Conics
{
    public class Ellipse : ConicSection
    {
        public Ellipse(ConicSection conicSection, Point3d pt1, Point3d pt2)
            : base(conicSection)
        {
            Point3dList pointList = new Point3dList(new[] { pt1, pt2 });
            EquationTransform.TryGetInverse(out Transform eqTransformInv);

            pointList.Transform(eqTransformInv);

            var ellipseMatrixValues = new double[2][];
            Vector<double> ellipseVector = Vector.Build.Dense(2, 1);

            for (int i = 0; i < 2; i++)
            {
                ellipseMatrixValues[i] = new[] { Math.Pow(pointList[i].X, 2), Math.Pow(pointList[i].Y, 2) };
            }

            Matrix<double> matrix = DenseMatrix.OfRowArrays(ellipseMatrixValues);
            //var solution = matrix.Solve(ellipseVector);
            var solution = matrix.SolveIterative(ellipseVector, new MlkBiCgStab());

            EllipseA = Math.Sqrt(1 / solution[0]);
            EllipseB = Math.Sqrt(1 / solution[1]);

            Plane basePlane = Plane.WorldXY;
            basePlane.Transform(EquationTransform);

            var ellipse = new Rhino.Geometry.Ellipse(basePlane, EllipseA, EllipseB);
            Section = ellipse.ToNurbsCurve();
        }

        public double EllipseA { get; private set; }

        public double EllipseB { get; private set; }


    }
}
