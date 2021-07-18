using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace the_Dominion
{
    public class ParabolaFromFocusComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ParabolaFromFocusComponent()
          : base("ConstructParabola", "CPrbF",
              "Constructs a Parabola from Focus",
              "Dominion", "Math")
        { }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Focus", "F", "The focus of the Parabola", GH_ParamAccess.item);
            pManager.AddPointParameter("Start", "S", "Start of the Parabola", GH_ParamAccess.item);
            pManager.AddPointParameter("End", "E", "End of the Parabola", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Parabola", "P", "The resulting Parabola", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d f = Point3d.Unset;
            Point3d p1 = Point3d.Unset;
            Point3d p2 = Point3d.Unset;

            DA.GetData(0, ref f);
            DA.GetData(1, ref p1);
            DA.GetData(2, ref p2);

            NurbsCurve parabola = NurbsCurve.CreateParabolaFromFocus(f, p1, p2);

            DA.SetData(0, parabola);
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
        public override Guid ComponentGuid => new Guid("73235e01-8fe6-4996-9074-50cc509ba68e");
    }

    public class ParabolaFromVertexComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ParabolaFromVertexComponent()
          : base("ConstructParabola", "CPrbV",
              "Constructs a Parabola from Vertex",
              "Dominion", "Math")
        { }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Vertex", "V", "The Vertex on the Parabola", GH_ParamAccess.item);
            pManager.AddPointParameter("Start", "S", "Start of the Parabola", GH_ParamAccess.item);
            pManager.AddPointParameter("End", "E", "End of the Parabola", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Parabola", "P", "The resulting Parabola", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d v = Point3d.Unset;
            Point3d p1 = Point3d.Unset;
            Point3d p2 = Point3d.Unset;

            DA.GetData(0, ref v);
            DA.GetData(1, ref p1);
            DA.GetData(2, ref p2);

            NurbsCurve parabola = NurbsCurve.CreateParabolaFromVertex(v, p1, p2);

            DA.SetData(0, parabola);
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
