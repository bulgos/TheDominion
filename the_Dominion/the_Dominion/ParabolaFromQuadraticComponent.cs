using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace the_Dominion
{
    public class ParabolaFromQuadraticComponent : GH_Component
    {
        public ParabolaFromQuadraticComponent()
          : base("ConstructParabolaFromQuadratic", "CPrbQ",
              "Constructs a Parabola of the form y = ax^2 + bx+ c",
              "Dominion", "Math")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane in which to create Parabola", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("A", "A", "A", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("B", "B", "B", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("C", "C", "C", GH_ParamAccess.item, 0);
            pManager.AddIntervalParameter("Domain", "D", "The Domain to calculate the function in", GH_ParamAccess.item, new Interval(-10, 10));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Parabola", "P", "The resulting Parabola", GH_ParamAccess.item);
            pManager.AddPointParameter("Focus", "F", "The Focal Point of the Parabola", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pl", "The calculated Base Plane of the Parabola", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane plane = Plane.Unset;
            double a = double.NaN;
            double b = double.NaN;
            double c = double.NaN;
            Interval interval = Interval.Unset;

            DA.GetData(0, ref plane);
            DA.GetData(1, ref a);
            DA.GetData(2, ref b);
            DA.GetData(3, ref c);
            DA.GetData(4, ref interval);

            Parabola parabola = new Parabola(plane, a, b, c, interval);

            DA.SetData(0, parabola.Section);
            DA.SetData(1, parabola.Focus);
            DA.SetData(2, parabola.BasePlane);
        }

        public override Guid ComponentGuid => new Guid("2df015c3-0faa-462e-b7e3-a6f4adcd5604");
    }
}
