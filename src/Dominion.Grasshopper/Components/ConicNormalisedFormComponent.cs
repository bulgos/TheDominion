using Grasshopper.Kernel;
using System;
using System.Drawing;
using Dominion.Conics.Wrappers;
using Dominion.Properties;
using Dominion.Core.Conics;

namespace Dominion.Conics.Components
{
    public class ConicNormalisedFormComponent : GH_Component
    {
        public ConicNormalisedFormComponent()
          : base("Normalise Conic", "NormConic",
              "Computes the standard/normalised form of the Conic by eliminating Transform from Equation",
              "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Conic", "C", "The Conic to Normalise", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Conic", "C", "The standard Conic", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ConicSection conicSection = null;

            DA.GetData(0, ref conicSection);

            DA.SetData(0, conicSection.WorldAlignedConic);
        }

        public override Guid ComponentGuid => new Guid("d445a951-5cb8-4d05-95bf-e51914570a62");

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.conic_normalise;
    }
}
