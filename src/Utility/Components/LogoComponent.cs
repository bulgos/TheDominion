using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace the_Dominion.Utility.Components
{
    public class LogoComponent : GH_Component
    {
        public LogoComponent()
            : base("Dominion Logo", "DomLogo", 
                  "Builds The Dominion Logo", 
                  "Dominion", "Logo")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane of Orientation", GH_ParamAccess.item);
            pManager.AddNumberParameter("Ratio", "R", "Height/Width Ratio of Logo", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Thickness", "T", "Thickness of Logo", GH_ParamAccess.item, 0.2);
            pManager.AddNumberParameter("Borders", "B", "Border curves of Logo", GH_ParamAccess.list);

            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Logo", "L", "The resulting Logo", GH_ParamAccess.item);
            pManager.AddCurveParameter("Borders", "B", "The Border Shapes", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.Unset;
            double ratio = double.NaN;
            double thickness = double.NaN;
            List<double> borderOffsets = new List<double>();

            DA.GetData(0, ref plane);
            DA.GetData(1, ref ratio);
            DA.GetData(2, ref thickness);
            DA.GetDataList(3, borderOffsets);

            Logo logo = new Logo(plane, ratio, thickness, borderOffsets);

            GH_Structure<GH_Curve> borders = new GH_Structure<GH_Curve>();

            for (int i = 0; i < logo.Borders.Count/2; i++)
            {
                GH_Path path = new GH_Path(i);

                borders.Append(new GH_Curve(logo.Borders[i * 2]), path);
                borders.Append(new GH_Curve(logo.Borders[i * 2 + 1]), path);
            }

            DA.SetData(0, logo.MainShape);
            DA.SetDataTree(1, borders);
        }

        public override Guid ComponentGuid => new Guid("2d9dae3d-cdb4-4f5a-9eba-f5d9b53e2296");
    }
}
