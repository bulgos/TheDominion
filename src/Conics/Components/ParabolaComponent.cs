using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using the_Dominion.Conics.Wrappers;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace the_Dominion.Conics.Components
{
    public class ParabolaComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ParabolaComponent()
          : base("ConstructParabola", "CPrb",
              "Constructs a Parabola in the form y = ax^2",
              "Dominion", "Math")
        { }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("a", "a", "a", GH_ParamAccess.item, 1);
            pManager.AddIntervalParameter("Domain", "D", "The Domain to calculate the function in", GH_ParamAccess.item, new Interval(-10, 10));
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
            double a = double.NaN;
            Interval interval = Interval.Unset;

            DA.GetData(0, ref a);
            DA.GetData(1, ref interval);

            Parabola parabola = new Parabola(a, interval);

            DA.SetData(0, parabola);
        }

        public override Guid ComponentGuid => new Guid("de9bbb6d-cb79-4db8-94ee-48d9045f34b0");
    }
}
