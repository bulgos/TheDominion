using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using the_Dominion.Conics.Wrappers;

namespace the_Dominion.Conics.Components
{
    public class ParabolaFromFourPointsComponent : GH_Component
    {
        public ParabolaFromFourPointsComponent()
          : base("ConstructParabolaFrom4Points", "CPrb4Pt",
              "Constructs Parabolae from 4 Points",
              "Dominion", "Conics")
        { }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("P1", "p1", "First point on the Parabola", GH_ParamAccess.item);
            pManager.AddPointParameter("P2", "p2", "Second point on the Parabola", GH_ParamAccess.item);
            pManager.AddPointParameter("P3", "p3", "Third point on the Parabola", GH_ParamAccess.item);
            pManager.AddPointParameter("P4", "p4", "Fourth point on the Parabola", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Parabola1", "P1", "The First Parabola", GH_ParamAccess.item);
            pManager.AddParameter(new Conic_Param(), "Parabola2", "P2", "The Second Parabola", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d p1 = Point3d.Unset;
            Point3d p2 = Point3d.Unset;
            Point3d p3 = Point3d.Unset;
            Point3d p4 = Point3d.Unset;

            DA.GetData(0, ref p1);
            DA.GetData(1, ref p2);
            DA.GetData(2, ref p3);
            DA.GetData(3, ref p4);

            if (p1 == Point3d.Unset ||
                p2 == Point3d.Unset ||
                p2 == Point3d.Unset ||
                p2 == Point3d.Unset)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Not enough inputs supplied to calculate Parabolae");
                return;
            }

            //Point3d p1 = new Point3d(-4, 4, 0);
            //Point3d p2 = new Point3d(-3, 2, 0);
            //Point3d p3 = new Point3d(0, 0, 0);
            //Point3d p4 = new Point3d(3, 1, 0);

            //Point3d p1 = new Point3d(-1, 4, 0);
            //Point3d p2 = new Point3d(-2, -2, 0);
            //Point3d p3 = new Point3d(0, 0, 0);
            //Point3d p4 = new Point3d(4, 0, 0);


            Parabola[] parabolae = Parabola.ComputeParabolasThroughFourPoints(p1, p2, p3, p4);

            if (parabolae == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Could not fit Parabolae through points given");
                return;
            }

            DA.SetData(0, parabolae[0]);
            DA.SetData(1, parabolae[1]);
        }

        public override Guid ComponentGuid => new Guid("e1fba840-9a49-48dc-bded-f2c056d144cc");
    }
}
