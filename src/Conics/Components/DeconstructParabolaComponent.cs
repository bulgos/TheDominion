using Grasshopper.Kernel;
using System;
using System.Linq;

namespace the_Dominion.Conics.Components
{
    public class DeconstructParabolaComponent : DeconstructConicComponent
    {
        public DeconstructParabolaComponent()
            : base("DeconstructParabola", "DPrb",
                  "Deconstruct a Parabola into its constituent parts",
                  "Dominion", "Conics")
        { }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            base.RegisterOutputParams(pManager);

            pManager.AddPlaneParameter("VertexPlane", "VPl", "The Plane the Conic was constructed from", GH_ParamAccess.item);
            pManager.AddNumberParameter("A", "A", "A", GH_ParamAccess.item);
            pManager.AddNumberParameter("B", "B", "B", GH_ParamAccess.item);
            pManager.AddNumberParameter("C", "C", "C", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Domain", "D", "The Domain to calculate the function in", GH_ParamAccess.item);
            pManager.AddNumberParameter("Discriminant", "Di", "The Discriminant", GH_ParamAccess.item);
            pManager.AddPointParameter("Roots", "R", "Roots of the Parabola", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            base.SolveInstance(DA);

            ConicSection conicSection = null;

            DA.GetData(0, ref conicSection);

            Parabola parabola = conicSection as Parabola;

            if (parabola == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Conic provided is not a Parabola");
                return;
            }

            DA.SetData(3, parabola.VertexPlane);
            DA.SetData(4, parabola.A);
            DA.SetData(5, parabola.B);
            DA.SetData(6, parabola.C);
            DA.SetData(7, parabola.Domain);
            DA.SetData(8, parabola.Discriminant);
            DA.SetDataList(9, parabola.Roots.Select(root => root.Item2));
        }

        public override Guid ComponentGuid => new Guid("71e49a09-9095-4ffd-9824-32eca6e0a9c3");
    }
}
