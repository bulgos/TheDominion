using Grasshopper.Kernel;
using System;

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
            pManager.AddNumberParameter("Width", "W", "Width of Logo", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Height", "H", "Height of Logo", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Thickness", "T", "Thickness of Logo", GH_ParamAccess.item, 0.2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Logo", "L", "The resulting Logo", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double width = double.NaN;
            double height = double.NaN;
            double thickness = double.NaN;

            DA.GetData(0, ref width);
            DA.GetData(1, ref height);
            DA.GetData(2, ref thickness);

            //Logo logo = new Logo();
            Logo logo = new Logo(width, height, thickness);

            DA.SetData(0, logo.Shape);
        }

        public override Guid ComponentGuid => new Guid("2d9dae3d-cdb4-4f5a-9eba-f5d9b53e2296");
    }
}
