﻿using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using Dominion.Conics.Wrappers;
using Dominion.Properties;
using Dominion.Core.Conics;

namespace Dominion.Conics.Components
{
    public class ParabolaFromQuadraticComponent : GH_Component
    {
        public ParabolaFromQuadraticComponent()
          : base("Construct Quadratic Parabola", "ConParabQ",
              "Constructs a Parabola of the form y = Ax² + Bx+ C",
              "Dominion", "Conics")
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
            pManager.AddParameter(new Conic_Param(), "Parabola", "P", "The resulting Parabola", GH_ParamAccess.item);
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

            Parabola parabola = new Parabola(a, b, c, plane, interval);

            DA.SetData(0, parabola);
        }

        public override Guid ComponentGuid => new Guid("2df015c3-0faa-462e-b7e3-a6f4adcd5604");

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override Bitmap Icon => Resources.parabola_quadratic;
    }
}
