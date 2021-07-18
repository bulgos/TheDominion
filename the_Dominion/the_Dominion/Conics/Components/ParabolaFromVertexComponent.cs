using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace the_Dominion
{
    public class ParabolaFromPointsComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ParabolaFromPointsComponent()
          : base("ConstructParabolaFrom3Points", "CPrb3Pt",
              "Constructs a Parabola from 3 Points",
              "Dominion", "Math")
        { }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane in which to create Parabola", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddPointParameter("P1", "p1", "First point on the Parabola", GH_ParamAccess.item);
            pManager.AddPointParameter("P2", "p2", "Second point on the Parabola", GH_ParamAccess.item);
            pManager.AddPointParameter("P3", "p3", "Third point on the Parabola", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Parabola", "P", "The resulting Parabola", GH_ParamAccess.item);
            pManager.AddPointParameter("Focus", "F", "The Focal Point of the Parabola", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pl", "The calculated Base Plane of the Parabola", GH_ParamAccess.item);
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

            var parabola = new Parabola(plane, p1, p2, p3);

            DA.SetData(0, parabola.Section);
            DA.SetData(1, parabola.Focus);
            DA.SetData(2, parabola.BasePlane);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("f9593814-9e59-4c86-b508-4c0d27510b69");
    }
}
