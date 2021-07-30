using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using the_Dominion.Utility;

namespace the_Dominion.Conics.Components
{
    public class GetOuterTangents : GH_Component
    {
        public GetOuterTangents()
            : base("OuterTangent", "OTan", 
                  "Computes the outer Tangents between two circles",
                  "Dominion", "Circles") { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCircleParameter("Circle1", "C1", "The first Circle", GH_ParamAccess.item);
            pManager.AddCircleParameter("Circle2", "C2", "The second Circle", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Tangent1", "T1", "The first Tangent", GH_ParamAccess.item);
            pManager.AddLineParameter("Tangent2", "T2", "The second Tangent", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Circle circle1 = Circle.Unset;
            Circle circle2 = Circle.Unset;

            DA.GetData(0, ref circle1);
            DA.GetData(1, ref circle2);

            var tangents = CircleMath.GetOuterTangets(circle1, circle2);

            DA.SetData(0, tangents[0]);
            DA.SetData(1, tangents[1]);
        }

        public override Guid ComponentGuid => new Guid("3df2a651-3a44-41b5-ae8b-2df306fd9cac");
    }
}
