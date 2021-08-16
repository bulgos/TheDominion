using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using the_Dominion.Conics.Wrappers;
using the_Dominion.Properties;

namespace the_Dominion.Conics.Components
{
    public class ParabolaFromThreePointsComponent : GH_Component
    {
        public ParabolaFromThreePointsComponent()
          : base("3-Point Parabola-Solver", "SolveParab3",
              "Solves a Parabola from 3 Points in the 2d Plane",
              "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane in which to create Parabola", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddPointParameter("P1", "p1", "First point on the Parabola", GH_ParamAccess.item, new Point3d(-1, 1, 0));
            pManager.AddPointParameter("P2", "p2", "Second point on the Parabola", GH_ParamAccess.item, new Point3d(0, 0, 0));
            pManager.AddPointParameter("P3", "p3", "Third point on the Parabola", GH_ParamAccess.item, new Point3d(1, 1, 0));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Parabola", "P", "The resulting Parabola", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.Unset;
            Point3d p1 = Point3d.Unset;
            Point3d p2 = Point3d.Unset;
            Point3d p3 = Point3d.Unset;

            DA.GetData(0, ref plane);
            DA.GetData(1, ref p1);
            DA.GetData(2, ref p2);
            DA.GetData(3, ref p3);

            var parabola = Parabola.From3Points(p1, p2, p3, plane, Interval.Unset);

            DA.SetData(0, parabola);
        }

        public override Guid ComponentGuid => new Guid("f9593814-9e59-4c86-b508-4c0d27510b69");

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override Bitmap Icon => Resources.parabola_3_point;
    }
}
