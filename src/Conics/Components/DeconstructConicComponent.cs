﻿using Grasshopper.Kernel;
using System;
using the_Dominion.Conics.Wrappers;

namespace the_Dominion.Conics.Components
{
    public class DeconstructConicComponent : GH_Component
    {
        public DeconstructConicComponent()
            : base("DeconstructConic", "DConic",
                  "Deconstruct a Conic into its constituent parts",
                  "Dominion", "Conics") { }

        protected DeconstructConicComponent(string name, string nickname, string description, string category, string subCategory)
            : base(name, nickname, description, category, subCategory) { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Conic_Param(), "Conic", "C", "The Conic to Deconstruct", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("ConicSection", "C", "The Conic Section", GH_ParamAccess.item);
            pManager.AddPointParameter("Focus1", "F1", "The first Conic Focus", GH_ParamAccess.item);
            pManager.AddPointParameter("Focus2", "F2", "The second Conic Focus", GH_ParamAccess.item);
            pManager.AddPlaneParameter("BasePlane", "Pl", "The Plane the Conic was constructed from", GH_ParamAccess.item);
            pManager.AddNumberParameter("A", "A", "A", GH_ParamAccess.item);
            pManager.AddNumberParameter("B", "B", "B", GH_ParamAccess.item);
            pManager.AddNumberParameter("C", "C", "C", GH_ParamAccess.item);
            pManager.AddNumberParameter("D", "D", "D", GH_ParamAccess.item);
            pManager.AddNumberParameter("E", "E", "E", GH_ParamAccess.item);
            pManager.AddNumberParameter("F", "F", "F", GH_ParamAccess.item);
            pManager.AddNumberParameter("Discriminant", "Di", "The Discriminant", GH_ParamAccess.item);
            pManager.AddTransformParameter("Transform", "X", "The Conic Transform", GH_ParamAccess.item);
            pManager.AddTextParameter("ConicFormatted", "Cf", "A formatted string representation of the Conic Equation", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ConicSection conicSection = null;
            
            DA.GetData(0, ref conicSection);

            if (conicSection == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No conic supplied to input.");
                return;
            }

            DA.SetData(0, conicSection.Section);
            DA.SetData(1, conicSection.Focus1);
            DA.SetData(2, conicSection.Focus2);
            DA.SetData(3, conicSection.BasePlane);
            DA.SetData(4, conicSection.A);
            DA.SetData(5, conicSection.B);
            DA.SetData(6, conicSection.C);
            DA.SetData(7, conicSection.D);
            DA.SetData(8, conicSection.E);
            DA.SetData(9, conicSection.F);
            DA.SetData(10, conicSection.ConicDiscriminant);
            DA.SetData(11, conicSection.TransformMatrix);
            DA.SetData(12, conicSection.FormatConicEquation());
        }

        public override Guid ComponentGuid => new Guid("040d0bab-ee94-4915-8f1a-b812d783048d");

        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
}
