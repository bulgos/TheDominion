﻿using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace the_Dominion.Conics.Components
{
    public class ConicSolverComponent : GH_Component
    {
        public ConicSolverComponent()
            : base("ConicSolver", "CSolv", 
                  "Solves a Conic",
                  "Dominion", "Conics")
        { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("P1", "p1", "First point on the Conic", GH_ParamAccess.item);
            pManager.AddPointParameter("P2", "p2", "Second point on the Conic", GH_ParamAccess.item);
            pManager.AddPointParameter("P3", "p3", "Third point on the Conic", GH_ParamAccess.item);
            pManager.AddPointParameter("P4", "p4", "Fourth point on the Conic", GH_ParamAccess.item);
            pManager.AddPointParameter("P5", "p5", "Fifth point on the Conic", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("A", "A", "A", GH_ParamAccess.item);
            pManager.AddNumberParameter("B", "B", "B", GH_ParamAccess.item);
            pManager.AddNumberParameter("C", "C", "C", GH_ParamAccess.item);
            pManager.AddNumberParameter("D", "D", "D", GH_ParamAccess.item);
            pManager.AddNumberParameter("E", "E", "E", GH_ParamAccess.item);
            pManager.AddNumberParameter("F", "F", "F", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d p1 = Point3d.Unset;
            Point3d p2 = Point3d.Unset;
            Point3d p3 = Point3d.Unset;
            Point3d p4 = Point3d.Unset;
            Point3d p5 = Point3d.Unset;

            DA.GetData(0, ref p1);
            DA.GetData(0, ref p2);
            DA.GetData(0, ref p3);
            DA.GetData(0, ref p4);
            DA.GetData(0, ref p5);

            ConicSolver conicSolver = ConicSolver.SolveConic(p1, p2, p3, p4, p5);

            DA.SetData(0, conicSolver.A);
            DA.SetData(1, conicSolver.B);
            DA.SetData(2, conicSolver.C);
            DA.SetData(3, conicSolver.D);
            DA.SetData(4, conicSolver.E);
            DA.SetData(5, conicSolver.F);
        }

        public override Guid ComponentGuid => new Guid("fc4c2115-1efc-4455-b016-6d3c717d5662");
    }
}