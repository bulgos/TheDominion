using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace the_Dominion.Utility.Components
{
    public class VectorAngleInPlaneComponent : GH_Component
    {
        public VectorAngleInPlaneComponent()
            : base("AngleInPlane", "VPl", 
                  "Computes the angle between a Vector and a Plane",
                  "Dominion", "Math")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "Pl", "The Plane to test against", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddVectorParameter("Vector", "V", "The Vector to test", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Angle", "A", "The Signed Angle between Plane and Vector", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.Unset;
            Vector3d vector = Vector3d.Unset;

            DA.GetData(0, ref plane);
            DA.GetData(1, ref vector);

            DA.SetData(0, vector.VectorAngleInPlane(plane));
        }

        public override Guid ComponentGuid => new Guid("1c9502c6-26ac-4ad6-9696-986173a6bffa");
    }
}
