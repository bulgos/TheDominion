using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using Dominion.Conics.Wrappers;
using Dominion.Properties;

namespace Dominion.Conics.Components
{
    public class HyperbolaComponent : GH_Component
    {
        public HyperbolaComponent()
          : base("Construct Hyperbola", "ConHyperb",
              "Constructs a Hyperbola in the form x² / A² - y² / B² = 1",
              "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane in which to create Hyperbola", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("A", "A", "a", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("B", "B", "b", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("h", "h", "height", GH_ParamAccess.item, 10);
            pManager.AddBooleanParameter("Flip", "F", "Flip the x and y axes of the Hyperbola\n" +
                "if true, Hyperbola will be created in the form y² / B² - x² / A² = 1", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Hyperbola", "H", "The resulting Hyperbola", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.Unset;
            double a = double.NaN;
            double b = double.NaN;
            double h = double.NaN;
            bool flip = false;

            DA.GetData(0, ref plane);
            DA.GetData(1, ref a);
            DA.GetData(2, ref b);
            DA.GetData(3, ref h);
            DA.GetData(4, ref flip);

            Hyperbola hyperbola = new Hyperbola(plane, a, b, h, flip);

            DA.SetData(0, hyperbola);
        }

        public override Guid ComponentGuid => new Guid("1c5e49a6-1290-4366-9216-2f1a9139fc0b");

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override Bitmap Icon => Resources.hyperbola_construct;
    }
}
