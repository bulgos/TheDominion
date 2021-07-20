using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace the_Dominion.Conics.Wrappers
{
    public class GH_Conic : GH_GeometricGoo<ConicSection>, IGH_PreviewData
    {
        public GH_Conic() { }

        public GH_Conic(ConicSection conicSection)
            : base(conicSection) { }

        public GH_Conic(GH_Conic gh_Conic)
            : this(gh_Conic.Value) { }

        public override BoundingBox Boundingbox =>
            Value == null
                ? BoundingBox.Empty
                : Value.BoundingBox;

        public BoundingBox ClippingBox => Boundingbox;

        public override string TypeName => ToString();

        public override string TypeDescription => $"Defines a Conic Section";

        public override bool CastFrom(object source)
        {
            if (source == null)
                return false;

            if (source is ConicSection conicSection)
            {
                Value = conicSection;
                return true;
            }

            return false;
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null)
                return;

            args.Pipeline.DrawCurve(Value.Section, args.Color);
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            if (Value == null)
                return null;

            return new GH_Conic(Value.Duplicate());
        }

        public override BoundingBox GetBoundingBox(Transform xform)
        {
            return Value.GetBoundingBox(xform);
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            if (Value == null)
                return null;

            ConicSection morphedConic = Value.Duplicate();
            morphedConic.Morph(xmorph);

            return new GH_Conic(morphedConic);
        }

        public override IGH_GeometricGoo Transform(Transform xform)
        {
            if (Value == null)
                return null;

            ConicSection transformedConic = Value.Duplicate();
            transformedConic.TransformShape(xform);

            return new GH_Conic(transformedConic);
        }

        public override object ScriptVariable()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value == null
                ? "Null Conic Section"
                : Value.ToString();
        }
    }
}
