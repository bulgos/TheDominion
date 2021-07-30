using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace the_Dominion.Conics.Components
{
    public class ConicTypeComponent : GH_Component
    {
        public ConicTypeComponent()
          : base("ConicType", "ConType",
              "Tests a curve to see if it is a valid type of conic section",
              "Dominion", "Utility")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to Test", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Conic Type", "T", "Type of Conic section", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            DA.GetData(0, ref curve);

            if (curve == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input C is not valid");
                return;
            }

            ConicSectionType conicType = curve.GetConicSectionType();

            DA.SetData(0, conicType.ToString());
        }

        public override Guid ComponentGuid => new Guid("786ab720-39a8-4694-9d08-be1ae47aa59d");

        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
}
