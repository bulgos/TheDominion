using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using the_Dominion.Conics.Wrappers;
using the_Dominion.Properties;

namespace the_Dominion.Conics.Components
{
    public class ParabolaFromThreePointsComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ParabolaFromThreePointsComponent()
          : base("3-Point Parabola-Solver", "SolveParab3",
              "Solves a Parabola from 3 Points in the 2d Plane",
              "Dominion", "Conics")
        { }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane in which to create Parabola", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddPointParameter("P1", "p1", "First point on the Parabola", GH_ParamAccess.item, new Point3d(-1, 1, 0));
            pManager.AddPointParameter("P2", "p2", "Second point on the Parabola", GH_ParamAccess.item, new Point3d(0, 0, 0));
            pManager.AddPointParameter("P3", "p3", "Third point on the Parabola", GH_ParamAccess.item, new Point3d(1, 1, 0));
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Parabola", "P", "The resulting Parabola", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.Unset;
            Point3d p1 = Point3d.Unset;
            Point3d p2 = Point3d.Unset;
            Point3d p3 = Point3d.Unset;

            DA.GetData(0, ref plane);
            DA.GetData(1, ref p1);
            DA.GetData(2, ref p2);
            DA.GetData(3, ref p3);

            var parabola = Parabola.From3Points(p1, p2, p3, plane, Interval.Unset);

            DA.SetData(0, parabola);
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("f9593814-9e59-4c86-b508-4c0d27510b69");

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override Bitmap Icon => Resources.parabola_3_point;
    }
}
