using Grasshopper.Kernel;
using System;
using System.Drawing;
using Dominion.Properties;

namespace Dominion.Utility.Components
{
#if DEBUG
    public class ACotangent : GH_Component
    {
        public ACotangent()
            : base("ACotangent", "ACot", 
                  "Finds the Inverse Cotangent",
                  "Dominion", "Math")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddAngleParameter("Angle", "A", "Angle to find ACot", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Rhino ACot", "R", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("ACot", "A", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("ACot Continuous", "C", "", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double angle = double.NaN;

            DA.GetData(0, ref angle);

            DA.SetData(0, Geometry.ACotRhino(angle));
            DA.SetData(1, Geometry.ACot(angle));
            DA.SetData(2, Geometry.ACotContinuous(angle));
        }

        public override Guid ComponentGuid => new Guid("4c924329-986a-462d-9e0d-38a61835940c");

        protected override Bitmap Icon => Resources.acotangent;
    }
#endif
}
