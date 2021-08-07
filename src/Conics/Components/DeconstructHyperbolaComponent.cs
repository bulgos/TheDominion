using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using the_Dominion.Conics.Wrappers;

namespace the_Dominion.Conics.Components
{
    public class DeconstructHyperbolaComponent : GH_Component
    {
        public DeconstructHyperbolaComponent()
            : base("DeconstructHyperbola", "DeHyperb",
                  "Deconstruct a Hyperbola into its constituent parts",
                  "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Hyperbola", "H", "The Hyperbola to Deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Hyperbola Section", "C", "The Hyperbola Section Curve", GH_ParamAccess.item);
            pManager.AddPointParameter("Control Points", "CP", "Control Points of the Hyperbola", GH_ParamAccess.list);
            pManager.AddNumberParameter("Axis A", "Aa", "Axis A of the Hyperbola", GH_ParamAccess.item);
            pManager.AddNumberParameter("Axis B", "Ab", "Axis B of the Hyperbola", GH_ParamAccess.item);
            pManager.AddPointParameter("Focus1", "F1", "The first Hyperbola Focus", GH_ParamAccess.item);
            pManager.AddPointParameter("Focus2", "F2", "The second Hyperbola Focus", GH_ParamAccess.item);
            pManager.AddPlaneParameter("BasePlane", "Pl", "The Plane the Hyperbola was constructed from", GH_ParamAccess.item);
            pManager.AddPointParameter("Apex", "Q", "Apex Point", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ConicSection conicSection = null;

            DA.GetData(0, ref conicSection);

            Hyperbola hyperbola = conicSection as Hyperbola;

            if (hyperbola == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Conic provided is not a Hyperbola");
                return;
            }
            if (!hyperbola.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Hyperbola is invalid, cannot deconstruct");
                return;
            }

            List<Point3d> controlPoints = new List<Point3d>();

            for (int i = 0; i < conicSection.Section.Points.Count; i++)
            {
                conicSection.Section.Points.GetPoint(i, out Point3d pt);
                controlPoints.Add(pt);
            }

            DA.SetData(0, hyperbola.Section);
            DA.SetDataList(1, controlPoints);
            DA.SetData(2, hyperbola.AxisA);
            DA.SetData(3, hyperbola.AxisB);
            DA.SetData(4, hyperbola.Focus1);
            DA.SetData(5, hyperbola.Focus2);
            DA.SetData(6, hyperbola.BasePlane);
            DA.SetData(7, hyperbola.Apex);
        }

        public override Guid ComponentGuid => new Guid("3305b5a2-5428-4b39-9a02-3f4d16e482f5");

        public override GH_Exposure Exposure => GH_Exposure.tertiary;
    }
}
