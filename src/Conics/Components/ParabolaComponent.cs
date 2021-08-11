using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using the_Dominion.Conics.Wrappers;
using the_Dominion.Properties;

namespace the_Dominion.Conics.Components
{
    public class ParabolaComponent : GH_Component
    {
        public ParabolaComponent()
          : base("Construct Parabola", "ConParab",
              "Constructs a Parabola in the form y = ax^2",
              "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("a", "a", "a", GH_ParamAccess.item, 1);
            pManager.AddIntervalParameter("Domain", "D", "The Domain to calculate the function in", GH_ParamAccess.item, new Interval(-10, 10));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Parabola", "P", "The resulting Parabola", GH_ParamAccess.item);
        }

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

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override Bitmap Icon => Resources.parabola_construct;
    }
}
