using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using the_Dominion.Conics.Wrappers;

namespace the_Dominion.Conics.Components
{
    public class DeconstructParabolaComponent : GH_Component
    {
        public DeconstructParabolaComponent()
            : base("Deconstruct Parabola", "DeParab",
                  "Deconstruct a Parabola into its constituent parts",
                  "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Parabola", "P", "The Parabola to Deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("ConicSection", "C", "The Conic Section", GH_ParamAccess.item);
            pManager.AddPointParameter("Control Points", "CP", "Control Points of the Parabola", GH_ParamAccess.list);
            pManager.AddPointParameter("Focus1", "F1", "The first Conic Focus", GH_ParamAccess.item);
            pManager.AddPlaneParameter("BasePlane", "Pl", "The Plane the Conic was constructed from", GH_ParamAccess.item);
            pManager.AddPlaneParameter("VertexPlane", "VPl", "The Plane the Conic was constructed from", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Domain", "D", "The Domain to calculate the function in", GH_ParamAccess.item);
            pManager.AddPointParameter("Roots", "R", "Roots of the Parabola", GH_ParamAccess.list);
            pManager.AddTextParameter("Direction", "D", "Parabola Direction", GH_ParamAccess.item);
            pManager.AddTextParameter("ConicFormatted", "Cf", "A formatted string representation of the Conic Equation", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ConicSection conicSection = null;

            DA.GetData(0, ref conicSection);

            Parabola parabola = conicSection as Parabola;

            if (parabola == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Conic provided is not a Parabola");
                return;
            }
            if (!parabola.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Parabola is invalid, cannot deconstruct");
                return;
            }

            string shape = parabola.ParabolaShape.ToString();
            shape = shape.Replace("Negative", "-");
            shape = shape.Replace("Positive", "+");

            List<Point3d> controlPoints = new List<Point3d>();

            for (int i = 0; i < conicSection.Section.Points.Count; i++)
            {
                conicSection.Section.Points.GetPoint(i, out Point3d pt);
                controlPoints.Add(pt);
            }

            DA.SetData(0, conicSection.Section);
            DA.SetDataList(1, controlPoints);
            DA.SetData(2, conicSection.Focus1);
            DA.SetData(3, conicSection.BasePlane);
            DA.SetData(4, parabola.VertexPlane);
            DA.SetData(5, parabola.Domain);
            DA.SetDataList(6, parabola.Roots);
            DA.SetData(7, shape);
            DA.SetData(8, conicSection.FormatConicEquation());
        }

        public override Guid ComponentGuid => new Guid("71e49a09-9095-4ffd-9824-32eca6e0a9c3");

        public override GH_Exposure Exposure => GH_Exposure.secondary;
    }
}
