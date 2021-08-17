using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;

namespace the_Dominion.Conics.Wrappers
{
    public class GH_Conic : GH_GeometricGoo<ConicSection>, IGH_PreviewData, IGH_BakeAwareData
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

        public override bool CastTo<Q>(out Q target)
        {
            if (typeof(Q).IsAssignableFrom(typeof(ConicSection)))
            {
                object conic = Value;
                target = (Q)conic;
                return true;
            }

            if (Value.IsValid)
            {
                if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
                {
                    object crv = new GH_Curve(Value.Section[0]);
                    target = (Q)crv;
                    return true;
                }

                if (typeof(Q).IsAssignableFrom(typeof(Curve)))
                {
                    object crv = Value.Section;
                    target = (Q)crv;
                    return true;
                }
            }

            target = default(Q);
            return false;
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null || !Value.IsValid)
                return;

            foreach (var section in Value.Section)
            {
                args.Pipeline.DrawCurve(section, args.Color, args.Thickness);
            }

            args.Pipeline.DrawPoint(Value.Focus1, Rhino.Display.PointStyle.RoundControlPoint, 5, args.Color);
            args.Pipeline.DrawPoint(Value.Focus2, Rhino.Display.PointStyle.RoundControlPoint, 5, args.Color);
        }

        public bool BakeGeometry(RhinoDoc doc, ObjectAttributes att, out Guid obj_guid)
        {
            obj_guid = Guid.Empty;

            if (!Value.IsValid)
                return false;

            foreach (var section in Value.Section)
            {
                obj_guid = doc.Objects.AddCurve(section);
            }

            return true;
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            if (Value == null)
                return null;

            return new GH_Conic(DuplicateConic());
        }

        private ConicSection DuplicateConic()
        {
            ConicSection conic = null;

            if (Value is Ellipse ellipse)
                conic = ellipse.Duplicate();
            else if (Value is Hyperbola hyperbola)
                conic = hyperbola.Duplicate();
            else if (Value is Parabola parabola)
                conic = parabola.Duplicate();

            return conic;
        }

        public override BoundingBox GetBoundingBox(Transform xform)
        {
            return Value.GetBoundingBox(xform);
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            if (Value == null)
                return null;

            ConicSection morphedConic = DuplicateConic();
            morphedConic.Morph(xmorph);

            return new GH_Conic(morphedConic);
        }

        public override IGH_GeometricGoo Transform(Transform xform)
        {
            if (Value == null)
                return null;

            ConicSection transformedConic = DuplicateConic();
            transformedConic.Transform(xform, true, true, true);

            return new GH_Conic(transformedConic);
        }

        public override object ScriptVariable()
        {
            return Value;
        }

        public override string ToString()
        {
            if (Value == null)
                return "Null Conic Section";

            if (!Value.IsValid)
                return $"Invalid {Value}";

            return Value.ToString();
        }
    }
}
