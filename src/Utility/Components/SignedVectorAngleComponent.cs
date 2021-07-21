using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace the_Dominion.Utility.Components
{
    public class SignedVectorAngleComponent : GH_Component
    {
        public SignedVectorAngleComponent()
            : base("SignedVectorAngle", "SVecAng",
                  "Computes the signed angle between two vectors",
                  "Dominion", "Math")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "Pl", "The Plane to test against", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddVectorParameter("Vector1", "V1", "The first Vector to test", GH_ParamAccess.item);
            pManager.AddVectorParameter("Vector2", "V2", "The second Vector to test", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Angle", "A", "The Signed Angle between the two Vectors in the given Plane", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.Unset;
            Vector3d vector1 = Vector3d.Unset;
            Vector3d vector2 = Vector3d.Unset;

            DA.GetData(0, ref plane);
            DA.GetData(1, ref vector1);
            DA.GetData(2, ref vector2);

            DA.SetData(0, Geometry.SignedVectorAngle(plane, vector1, vector2));
        }

        public override Guid ComponentGuid => new Guid("6d9594fb-c8a7-4876-ab54-264346e4b0e6");
    }
}
