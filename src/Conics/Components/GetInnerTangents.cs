using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using the_Dominion.Utility;

namespace the_Dominion.Conics.Components
{
    public class GetInnerTangents : GH_Component
    {
        public GetInnerTangents()
            : base("InnerTangent", "ITan", 
                  "Computes the inner Tangents between two circles",
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

            var tangents = CircleMath.GetInnerTangents(circle1, circle2);

            DA.SetData(0, tangents[0]);
            DA.SetData(1, tangents[1]);
        }

        public override Guid ComponentGuid => new Guid("9a5fed88-60bd-4db7-a1ab-dbcb0352b33a");
    }
}
