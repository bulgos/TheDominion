using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Dominion.Conics.Wrappers;
using Dominion.Properties;
using System.Drawing;

namespace Dominion.Conics.Components
{
    public class ConicFrom4PointsComponent : GH_Component
    {
        public ConicFrom4PointsComponent()
            : base("4-Point Conic-Solver", "SolveConic4",
                  "Solves a Conic from 4 points in the XY Plane Aligned To Axes",
                  "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane in which to create Parabola", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddPointParameter("Points", "P", "Points on the Conic", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Conic", "C", "The conic section through 5 points", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.Unset;
            List<Point3d> pts = new List<Point3d>();

            DA.GetData(0, ref plane);
            DA.GetDataList(1, pts);

            ConicSection conic = ConicSection.From4Points(pts, plane);

            DA.SetData(0, conic);
        }

        public override Guid ComponentGuid => new Guid("2172aea6-b238-4afe-bf87-376bf38d20af");
        
        protected override Bitmap Icon => Resources.conic_4_point;
    }
}
