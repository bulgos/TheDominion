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
    public class HyperbolaComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public HyperbolaComponent()
          : base("ConstructHyperbola", "CHyprb",
              "Constructs a Hyperbola in the form x^2 / a^2 - y^2 / b^2 = 1",
              "Dominion", "Math")
        { }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane in which to create Hyperbola", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("a", "a", "a", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("b", "b", "b", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("h", "h", "height", GH_ParamAccess.item, 10);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Hyperbola", "H", "The resulting Hyperbola", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.Unset;
            double a = double.NaN;
            double b = double.NaN;
            double h = double.NaN;

            DA.GetData(0, ref plane);
            DA.GetData(1, ref a);
            DA.GetData(2, ref b);
            DA.GetData(3, ref h);

            NurbsCurve hyperbola = Hyperbola.ComputeHyperbola(a, b, h);
            
            Transform xform = Transform.PlaneToPlane(Plane.WorldXY, plane);
            hyperbola.Transform(xform);

            DA.SetData(0, hyperbola);
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("1c5e49a6-1290-4366-9216-2f1a9139fc0b");
    }
}
