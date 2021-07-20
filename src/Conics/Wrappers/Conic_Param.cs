using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace the_Dominion.Conics.Wrappers
{
    public class Conic_Param : GH_PersistentGeometryParam<GH_Conic>, IGH_PreviewObject
    {
        public Conic_Param()
            : base(new GH_InstanceDescription(
                "ConicSection", "Conic",
                "Maintains a collection of Conic Sections",
                "Params", "Geometry"
                )) { }

        public override Guid ComponentGuid => new Guid("66a13bbe-6cd2-4501-888e-d3b324997594");

        public bool Hidden { get; set; } = false;

        public bool IsPreviewCapable => true;

        public BoundingBox ClippingBox => Preview_ComputeClippingBox();

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        public void DrawViewportMeshes(IGH_PreviewArgs args) { }

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            Preview_DrawWires(args);
        }

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Conic> values)
        {
            return GH_GetterResult.cancel;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Conic value)
        {
            return GH_GetterResult.cancel;
        }
    }
}
