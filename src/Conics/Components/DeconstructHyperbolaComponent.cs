﻿using Grasshopper.Kernel;
using System;

namespace the_Dominion.Conics.Components
{
    public class DeconstructHyperbolaComponent : DeconstructConicComponent
    {
        public DeconstructHyperbolaComponent()
            : base("DeconstructHyperbola", "DHyprb",
                  "Deconstruct a Hyperbola into its constituent parts",
                  "Dominion", "Conics")
        { }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            base.RegisterOutputParams(pManager);

            pManager.AddNumberParameter("HyperbolaA", "hA", "The A term from the standard Hyperbola form, different from Conic.A", GH_ParamAccess.item);
            pManager.AddNumberParameter("Hyperbolab", "hB", "The B term from the standard Hyperbola form, different from Conic.B", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            base.SolveInstance(DA);

            ConicSection conicSection = null;

            DA.GetData(0, ref conicSection);

            Hyperbola hyperbola = conicSection as Hyperbola;

            if (hyperbola == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Conic provided is not a Hyperbola");
                return;
            }

            DA.SetData(12, hyperbola.HyperbolaA);
            DA.SetData(13, hyperbola.HyperbolaB);
        }

        public override Guid ComponentGuid => new Guid("3305b5a2-5428-4b39-9a02-3f4d16e482f5");

        public override GH_Exposure Exposure => GH_Exposure.tertiary;
    }
}