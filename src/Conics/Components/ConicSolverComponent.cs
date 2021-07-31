using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using the_Dominion.Conics.Wrappers;

namespace the_Dominion.Conics.Components
{
    public class ConicSolverComponent : GH_Component
    {
        public ConicSolverComponent()
            : base("ConicSolver", "CSolv", 
                  "Solves a Conic in the 2d Plane",
                  "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("P1", "p1", "First point on the Conic", GH_ParamAccess.item);
            pManager.AddPointParameter("P2", "p2", "Second point on the Conic", GH_ParamAccess.item);
            pManager.AddPointParameter("P3", "p3", "Third point on the Conic", GH_ParamAccess.item);
            pManager.AddPointParameter("P4", "p4", "Fourth point on the Conic", GH_ParamAccess.item);
            pManager.AddPointParameter("P5", "p5", "Fifth point on the Conic", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Conic", "C", "The conic section through 5 points", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d p1 = Point3d.Unset;
            Point3d p2 = Point3d.Unset;
            Point3d p3 = Point3d.Unset;
            Point3d p4 = Point3d.Unset;
            Point3d p5 = Point3d.Unset;

            DA.GetData(0, ref p1);
            DA.GetData(1, ref p2);
            DA.GetData(2, ref p3);
            DA.GetData(3, ref p4);
            DA.GetData(4, ref p5);

            var pts = new[] { p1, p2, p3, p4, p5 };
            ConicSection conicSection = ConicSection.From5Points(pts);

            DA.SetData(0, conicSection);
        }

        public override Guid ComponentGuid => new Guid("fc4c2115-1efc-4455-b016-6d3c717d5662");

        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
}
