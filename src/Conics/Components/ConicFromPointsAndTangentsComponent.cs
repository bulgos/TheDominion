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
    public class ConicFromPointsAndTangentsComponent : GH_Component
    {
        public ConicFromPointsAndTangentsComponent()
            : base("Tangent-Point Conic", "ConicTP",
                  "Builds a conic from a mix of 5 points and/or tangents",
                  "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Points on the Conic", GH_ParamAccess.list);
            pManager.AddLineParameter("Tangents", "T", "Tangents to the Conic", GH_ParamAccess.list);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Conic", "C", "The conic section through 5 points", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> pts = new List<Point3d>();
            List<Line> lines = new List<Line>();

            DA.GetDataList(0, pts);
            DA.GetDataList(1, lines);

            ConicSection conic = ConicSection.From5PointsAndTangents(pts, lines);

            DA.SetData(0, conic);
        }

        public override Guid ComponentGuid => new Guid("98a5d7b6-b32e-4c95-8a6c-9a2b2bc0cb8a");
    }
}
