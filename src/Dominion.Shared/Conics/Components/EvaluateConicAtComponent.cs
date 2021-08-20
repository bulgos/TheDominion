using Grasshopper.Kernel;
using System;
using System.Drawing;
using Dominion.Conics.Wrappers;
using Dominion.Properties;

namespace Dominion.Conics.Components
{
    public class EvaluateConicAtComponnent : GH_Component
    {
        public EvaluateConicAtComponnent()
            :base("Evaluate Conic At", "EvalConic",
                 "Evaluates the conic Equation at the given position",
                 "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Conic", "C", "Conic Section to Evaluate", GH_ParamAccess.item);
            pManager.AddNumberParameter("XPostion", "X", "X Position to evaluate on Conic", GH_ParamAccess.item);
            pManager.AddNumberParameter("YPostion", "Y", "Y Position to evaluate on Conic", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("PointsX", "PX", "Points at X-value", GH_ParamAccess.list);
            pManager.AddPointParameter("PointsY", "PY", "Points at Y-value", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ConicSection conicSection = null;
            double x = double.NaN;
            double y = double.NaN;

            DA.GetData(0, ref conicSection);
            DA.GetData(1, ref x);
            DA.GetData(2, ref y);

            var xPoints = conicSection.ComputePointAtX(x);
            var yPoints = conicSection.ComputePointAtY(y);
            
            if (xPoints.Length > 0)
                DA.SetDataList(0, xPoints);

            if (yPoints.Length > 0)
                DA.SetDataList(1, yPoints);
        }

        public override Guid ComponentGuid => new Guid("61eaaf21-fcda-420f-8f9b-6eb2a73727ee");

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.conic_evaluate;
    }
}
