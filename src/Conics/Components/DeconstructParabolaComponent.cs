using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using the_Dominion.Conics.Wrappers;
using the_Dominion.Properties;

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
            pManager.AddCurveParameter("ParabolaSection", "C", "The Parabola Section Curve", GH_ParamAccess.list);
            pManager.AddPointParameter("Control Points", "CP", "Control Points of the Parabola", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Axis A", "Aa", "Axis A of the Parabola (if existant)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Axis B", "Ab", "Axis B of the Parabola (if existant)", GH_ParamAccess.item);
            pManager.AddPointParameter("Focus", "F", "The Parabola Focus", GH_ParamAccess.item);
            pManager.AddPlaneParameter("BasePlane", "Pl", "The Plane the Conic was constructed from", GH_ParamAccess.item);
            pManager.AddPlaneParameter("VertexPlane", "VPl", "The Plane the Parabola was constructed from", GH_ParamAccess.item);
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

            GH_Structure<GH_Point> controlPoints = new GH_Structure<GH_Point>();

            for (int i = 0; i < conicSection.Section.Count; i++)
            {
                if (conicSection.Section[i] is NurbsCurve nurbsCurve)
                {
                    GH_Path path = new GH_Path(i);

                    for (int j = 0; j < nurbsCurve.Points.Count; j++)
                    {
                        nurbsCurve.Points.GetPoint(j, out Point3d pt);
                        controlPoints.Append(new GH_Point(pt), path);
                    }
                }
            }

            DA.SetDataList(0, parabola.Section);
            DA.SetDataTree(1, controlPoints);
            DA.SetData(2, parabola.AxisA);
            DA.SetData(3, parabola.AxisB);
            DA.SetData(4, parabola.Focus1);
            DA.SetData(5, parabola.BasePlane);
            DA.SetData(6, parabola.VertexPlane);
            DA.SetData(7, parabola.Domain);
            DA.SetDataList(8, parabola.Roots);
            DA.SetData(9, shape);
            DA.SetData(10, parabola.FormatConicEquation());
        }

        public override Guid ComponentGuid => new Guid("71e49a09-9095-4ffd-9824-32eca6e0a9c3");

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override Bitmap Icon => Resources.parabola_deconstruct;
    }
}
