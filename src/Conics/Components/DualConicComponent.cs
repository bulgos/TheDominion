using Grasshopper.Kernel;
using System;
using the_Dominion.Conics.Wrappers;

namespace the_Dominion.Conics.Components
{
    public class DualConicComponent : GH_Component
    {
        public DualConicComponent()
            : base("Dual Conic", "DuaCon",
                  "Solves for the Dual Conic",
                  "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Conic", "C", "Conic to find dual of", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Conic", "C", "Dual Conic", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ConicSection conic = null;

            DA.GetData(0, ref conic);

            DA.SetData(0, conic.Dual);
        }

        public override Guid ComponentGuid => new Guid("5be1af6a-aed6-4752-9246-e3e3cd040e9d");

        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
}
