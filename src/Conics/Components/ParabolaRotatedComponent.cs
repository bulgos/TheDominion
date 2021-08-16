using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using the_Dominion.Conics.Wrappers;
using the_Dominion.Properties;

namespace the_Dominion.Conics.Components
{
    public class ParabolaRotatedComponent : GH_Component
    {
        public ParabolaRotatedComponent()
          : base("Construct Rotated Parabola", "ConRParab",
              "Constructs a Parabola with 2d rotation",
              "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("A", "A", "A", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("B", "B", "B", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("C", "C", "C", GH_ParamAccess.item, 0);
            pManager.AddAngleParameter("Rotation", "R", "Angle to Rotate the Parabola", GH_ParamAccess.item, -0.5 * Math.PI);
            pManager.AddIntervalParameter("Domain", "D", "The Domain to calculate the function in", GH_ParamAccess.item, new Interval(-5, 5));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Parabola", "P", "The resulting Parabola", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double a = double.NaN;
            double b = double.NaN;
            double c = double.NaN;
            double angle = double.NaN;
            Interval interval = Interval.Unset;

            DA.GetData(0, ref a);
            DA.GetData(1, ref b);
            DA.GetData(2, ref c);
            DA.GetData(3, ref angle);
            DA.GetData(4, ref interval);

            Plane plane = Plane.WorldXY;
            plane.Rotate(angle, plane.ZAxis);

            Parabola parabola = new Parabola(a, b, c, plane, interval);

            DA.SetData(0, parabola);
        }

        public override Guid ComponentGuid => new Guid("b50eea9d-4e9b-470f-bc26-47667d9bdc79");

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override Bitmap Icon => Resources.parabola_rotated;
    }
}
