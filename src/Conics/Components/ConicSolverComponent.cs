using Grasshopper.Kernel;
using System;
using System.Drawing;
using the_Dominion.Conics.Wrappers;
using the_Dominion.Properties;

namespace the_Dominion.Conics.Components
{
    public class ConicSolverComponent : GH_Component
    {
        public ConicSolverComponent()
            : base("Conic-Solver", "CSolve",
                  "Solves a Conic in the 2d Plane",
                  "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("A", "A", "Ax²", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("B", "B", "Bxy", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("C", "C", "Cy²", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("D", "D", "Dx", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("E", "E", "Ey", GH_ParamAccess.item, -1);
            pManager.AddNumberParameter("F", "F", "F", GH_ParamAccess.item, 0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Conic", "C", "The conic section through 5 points", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double a = double.NaN;
            double b = double.NaN;
            double c = double.NaN;
            double d = double.NaN;
            double e = double.NaN;
            double f = double.NaN;

            DA.GetData(0, ref a);
            DA.GetData(1, ref b);
            DA.GetData(2, ref c);
            DA.GetData(3, ref d);
            DA.GetData(4, ref e);
            DA.GetData(5, ref f);

            ConicSection conicSection = ConicSection.FromConicEquation(a, b, c, d, e, f);

            DA.SetData(0, conicSection);
        }

        public override Guid ComponentGuid => new Guid("4223dc7c-1790-4ce5-9d44-e013f1ab3f2c");

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.conic_solver;
    }
}
