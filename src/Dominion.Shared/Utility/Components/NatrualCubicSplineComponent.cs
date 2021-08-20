using Dominion.Properties;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Dominion.Utility.Components
{
#if V7
    public class NatrualCubicSplineComponent : GH_Component
    {
        public NatrualCubicSplineComponent()
            : base("Natural Cubic Spline", "NatSpline",
                  "Creates a Natural Cubic Spline from a given NurbsCurve",
                  "Dominion", "Splines")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Points to define Natural Cubic Spline", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Interpolate", "I", "If true, curve will pass through points. Else points will be used as control points", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Periodic", "P", "Is Curve Periodic or not", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Natural Spline", "N", "The Natural Cubic Spline", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();
            bool isPeriodic = false;
            bool interpolate = true;

            DA.GetDataList(0, points);
            DA.GetData(1, ref interpolate);
            DA.GetData(2, ref isPeriodic);

            if (points.Count < 4)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Not enough points supplied to create curve");
                return;
            }

            var nurbsCurve = NurbsCurve.CreateSubDFriendly(points, interpolate, isPeriodic);

            DA.SetData(0, nurbsCurve);
        }

        public override Guid ComponentGuid => new Guid("9c4ed386-a6e9-4ee6-8033-3d40c34a419e");

        protected override Bitmap Icon => Resources.conic_4_point;
    }
#endif
}