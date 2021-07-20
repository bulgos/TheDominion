using Grasshopper.Kernel;
using System;
using the_Dominion.Conics.Wrappers;

namespace the_Dominion.Conics.Components
{
    public class DeconstructConicComponent : GH_Component
    {
        public DeconstructConicComponent()
            : base("DeconstructConic", "DConic",
                  "Deconstruct a Conic into its constituent parts",
                  "Dominion", "Conics") { }

        protected DeconstructConicComponent(string name, string nickname, string description, string category, string subCategory)
            : base(name, nickname, description, category, subCategory) { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Parabola", "P", "The Parabola to Deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("ConicSection", "C", "The Conic Section", GH_ParamAccess.item);
            pManager.AddPointParameter("Focus", "F", "The Conic Focus", GH_ParamAccess.item);
            pManager.AddPlaneParameter("BasePlane", "Pl", "The Plane the Conic was constructed from", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ConicSection conicSection = null;
            
            DA.GetData(0, ref conicSection);

            DA.SetData(0, conicSection.Section);
            DA.SetData(1, conicSection.Focus);
            DA.SetData(2, conicSection.BasePlane);
        }

        public override Guid ComponentGuid => new Guid("040d0bab-ee94-4915-8f1a-b812d783048d");
    }
}
